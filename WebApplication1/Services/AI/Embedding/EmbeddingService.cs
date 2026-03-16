using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services.AI.Embedding
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public EmbeddingService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<float[]> CreateEmbeddingAsync(string query)
        {
            return await GenerateEmbeddingAsync(query);
        }

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            var apiKey = _config["HuggingFaceAI:ApiKey"];
            var model = _config["HuggingFaceAI:EmbeddingModel"];

            Console.WriteLine(apiKey + " " + model);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://router.huggingface.co/hf-inference/models/{model}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var body = JsonSerializer.Serialize(new
            {
                inputs = text
            });

            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Embedding error: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var embedding = JsonSerializer.Deserialize<float[]>(json);

            return embedding;
        }
    }
}