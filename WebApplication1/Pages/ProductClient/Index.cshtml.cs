using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.AI;
using WebApplication1.Services.AI.RagService;
using WebApplication1.Services.RedisManagement;
using WebApplication1.Services.ServiceDAO;
using static WebApplication1.Pages.ChatAI.IndexModel;

namespace  WebApplication1.Pages.ProductClient
{
    public class IndexModel : PageModel
    {
        private readonly WebApplication1Context _context;
        private readonly ICartService _cartService;
        private readonly ProduitDAO _ProductService;
        private readonly IOrchestratorRagService _RagService;

        public IndexModel(   WebApplication1Context context,
                             ICartService cartService,
                             ProduitDAO productService ,
                             IOrchestratorRagService orchestratorRag )      
        {
            _context = context;
            _cartService = cartService;
            _ProductService = productService;
            _RagService = orchestratorRag;
        }


        [BindProperty]
        public string Question { get; set; } = "";

        public string Response { get; set; } = "";

        // Optionnel : données structurées extraites depuis la réponse LLM (pour debug / usage interne)
        public StructuredData? ParsedStructuredData { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Product> Products { get; set; } = new();  
        public int CartCount { get; set; }


        // Affichage des produits et des catégories
        public async Task OnGetAsync(int? categoryId = -1)
        {
            Categories = await _context.Category.ToListAsync();

            if (categoryId.HasValue)
            {
                if (categoryId == -1)
                {
                    Products = await _context.Product
                        .Include(p => p.Category)
                        .ToListAsync();
                }
                else
                {
                    Products = await _context.Product
                        .Where(p => p.CategoryId == categoryId)
                        .Include(p => p.Category)
                        .ToListAsync();
                }
            }else
            {
                Products = await _context.Product
                   .Where(p => p.CategoryId == 1)
                    .Include(p => p.Category)
                    .ToListAsync();
            }

                CartCount = await _cartService.GetTotalQuantityAsync();
        }

        // Gestion de la question posée à l'IA
        public async Task<IActionResult> OnPostAsync()
        {
            // Récupérer produits & panier
            List<Product> products = await _ProductService.GetAllProduitsAsync();
            Dictionary<int, CartItemCache> cartItems = await _cartService.GetCartAsync();

            // Appel RAG : LlmService gère la récupération des produits similaires et la construction du prompt
            var raw = await _RagService.AskAsync(Question);

            // Extraire la partie visible (lisible) et le JSON structuré (si présent)
            var (visible, structured) = ExtractVisibleAndStructuredJson(raw);

            // Nettoyage simple pour améliorer lisibilité (supprime retours de code inutiles, trim)
            visible = NormalizeWhitespaceForDisplay(visible);

            Response = visible;
            ParsedStructuredData = structured;

            return Page();
        }

        private static (string visible, StructuredData? structured) ExtractVisibleAndStructuredJson(string llmRaw)
        {
            if (string.IsNullOrWhiteSpace(llmRaw))
                return (string.Empty, null);

            // 1) Cherche un bloc ```json ... ``` (le plus courant)
            var jsonFencePattern = new Regex("```json\\s*(\\{[\\s\\S]*?\\})\\s*```", RegexOptions.IgnoreCase);
            var m = jsonFencePattern.Match(llmRaw);
            if (m.Success)
            {
                var jsonText = m.Groups[1].Value;
                var visible = llmRaw.Substring(0, m.Index).Trim();
                var structured = TryParseStructured(jsonText);
                return (visible, structured);
            }

            // 2) Sinon cherche le dernier '{' dans la réponse et tente de parser jusqu'à la fin
            var lastBrace = llmRaw.LastIndexOf('{');
            if (lastBrace >= 0)
            {
                var candidate = llmRaw.Substring(lastBrace);
                var visible = llmRaw.Substring(0, lastBrace).Trim();

                var structured = TryParseStructured(candidate);
                if (structured != null)
                    return (visible, structured);
            }

            // 3) Aucun JSON structuré trouvé -> retourner le texte complet comme visible
            return (llmRaw.Trim(), null);
        }

        private static StructuredData? TryParseStructured(string jsonText)
        {
            try
            {
                using var doc = JsonDocument.Parse(jsonText);
                var root = doc.RootElement;

                var structured = new StructuredData
                {
                    Confidence = root.TryGetProperty("confidence", out var c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null,
                    TotalPanier = root.TryGetProperty("total_panier", out var t) && (t.ValueKind == JsonValueKind.Number) ? t.GetDecimal() : null,
                    RecommendedProducts = root.TryGetProperty("recommended_products", out var r) && r.ValueKind == JsonValueKind.Array
                        ? r.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToList()
                        : new List<string>(),
                    Sources = root.TryGetProperty("sources", out var s) && s.ValueKind == JsonValueKind.Array
                        ? s.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.String).Select(e => e.GetString()!).ToList()
                        : new List<string>()
                };

                return structured;
            }
            catch
            {
                return null;
            }
        }

        private static string NormalizeWhitespaceForDisplay(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Supprime les backticks restants et normalise les doubles espaces
            var cleaned = text.Replace("```", "").Trim();

            // Assure qu'il y a des retours à la ligne entre phrases longues pour meilleure lisibilité
            // (ne modifie pas les listes déjà formatées)
            // Remplace " . " mal formatté par ".\n\n" pour séparer paragraphes si nécessaire
            cleaned = Regex.Replace(cleaned, @"\s*\.\s+", ".\n\n");

            // Supprime éventuels espaces multiples
            cleaned = Regex.Replace(cleaned, @"[ \t]{2,}", " ");

            return cleaned.Trim();
        }

        public class StructuredData
        {
            public string? Confidence { get; set; }
            public decimal? TotalPanier { get; set; }
            public List<string> RecommendedProducts { get; set; } = new();
            public List<string> Sources { get; set; } = new();
        }

        // Ajouter au panier
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            await _cartService.AddToCartAsync(productId, quantity);
            return RedirectToPage();
        }
    }
}

