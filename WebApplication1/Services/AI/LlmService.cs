using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

 // commentaire enehjerj ejktrkrt trtiof fnfgjgfjfg
namespace WebApplication1.Services.AI
{
    public class LlmService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public LlmService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string> AskAsync(string userMessage, string context)
        {
            var apiKey = _config["HuggingFaceAI:ApiKey"];
            var model = _config["HuggingFaceAI:Model"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Clé API non configurée. Ajoutez 'AI:ApiKey' dans appsettings ou User Secrets.");
            }

            if (string.IsNullOrWhiteSpace(model) )
            {
                throw new InvalidOperationException("Model non configurée. Ajoutez 'AI:ApiKey' dans appsettings ou User Secrets.");
            }


            var requestBody = new
            {
                model,
                messages = new[]
                {
                    new { role = "system", content = context },
                    new { role = "user", content = userMessage }
                }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://router.huggingface.co/v1/chat/completions"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"HF error {response.StatusCode}: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString()!;
        }

        public async Task<string> AskWithRetrievalAsync(
            string userMessage,
            Dictionary<int, CartItemCache> cartItems,
            IEnumerable<Product> allProducts,
            int topK = 10)
        {
            // 1) Récupérer topK produits similaires (8-15)
            var retrievedProducts = ProductRetrievalService.RetrieveMostSimilar(allProducts, userMessage, topK);

            // 2) Préparer retrievedDocs (titre, contenu, source)
            var retrievedDocs = retrievedProducts.Select(p =>
                (Title: p.Name ?? $"Produit {p.Id}",
                 Content: $"{p.Description ?? string.Empty}",
                 Source: $"/ProductClient/Details?id={p.Id}")).ToList();

            // 3) Construire le prompt (ici : on passe uniquement les produits récupérés au lieu du catalogue complet)
            var prompt = PromptContext.GetEcommercePrompt(retrievedProducts.ToList(), cartItems, retrievedDocs);

            // 4) Appeler le LLM
            return await AskAsync(userMessage, prompt);
        }
    }
}