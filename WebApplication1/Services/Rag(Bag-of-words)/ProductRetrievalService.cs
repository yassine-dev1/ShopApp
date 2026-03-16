
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;


namespace WebApplication1.Services.AI
{
    /// <summary>
    /// Recherche approximative locale : calcule une similarité cosine sur TF (bag-of-words)
    /// entre la requête et la concaténation (name + description + category).
    /// Retourne entre 8 et 15 produits les plus similaires (par défaut 10).
    /// Remplacez par un vrai vector search pour meilleure qualité.
    /// </summary>
    public static class ProductRetrievalService
    {
        public static List<Product> RetrieveMostSimilar(IEnumerable<Product> products, string query, int requestedCount = 10)
        {
            if (products == null) return new List<Product>();

            // Clamp requestedCount between 8 and 15
            int count = Math.Clamp(requestedCount, 8, 15);

            var prodList = products.ToList();

            if (string.IsNullOrWhiteSpace(query))
            {
                // Si pas de requête, retourne simplement les premiers produits (ou un échantillon)
                return prodList.Take(count).ToList();
            }

            var qVec = TermFreqVector(query);

            var scored = new List<(Product product, double score)>();

            foreach (var p in prodList)
            {
                var combined = $"{p.Name} {p.Description} {p.Category?.Name}";
                var pVec = TermFreqVector(combined);
                var sim = CosineSimilarity(qVec, pVec);
                scored.Add((p, sim));
            }

            return scored
                .OrderByDescending(s => s.score)
                .ThenBy(s => s.product.Id)
                .Take(count)
                .Select(s => s.product)
                .ToList();
        }

        private static Dictionary<string, double> TermFreqVector(string text)
        {
            var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(text)) return dict;

            // tokenisation simple : letters + digits, sépare par tout le reste
            var tokens = text
                .ToLowerInvariant()
                .Split(new char[] { ' ', '\t', '\r', '\n', ',', '.', ';', ':', '-', '_', '/', '\\', '(', ')', '[', ']', '{', '}', '\"', '\'' },
                       StringSplitOptions.RemoveEmptyEntries);

            foreach (var t in tokens)
            {
                if (t.Length <= 1) continue; // ignorer tokens unitaires
                if (dict.TryGetValue(t, out var v)) dict[t] = v + 1;
                else dict[t] = 1;
            }

            return dict;
        }

        private static double CosineSimilarity(Dictionary<string, double> a, Dictionary<string, double> b)
        {
            if (a == null || b == null || a.Count == 0 || b.Count == 0) return 0.0;

            double dot = 0.0;
            foreach (var kv in a)
            {
                if (b.TryGetValue(kv.Key, out var bv))
                {
                    dot += kv.Value * bv;
                }
            }

            double normA = Math.Sqrt(a.Values.Sum(v => v * v));
            double normB = Math.Sqrt(b.Values.Sum(v => v * v));

            if (normA == 0 || normB == 0) return 0.0;

            return dot / (normA * normB);
        }
    }
}