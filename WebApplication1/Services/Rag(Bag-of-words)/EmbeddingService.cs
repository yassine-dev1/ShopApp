//using System.Net.Http.Headers;
//using System.Text;
//using System.Text.Json;

//namespace WebApplication1.Services.AI
//{
//    // Services/AI/Embeddings/EmbeddingService.cs
//    public class EmbeddingService
//    {
//        private readonly HttpClient _http;
//        private readonly string _apiKey;
//        private readonly string _modelUrl = "https://api-inference.huggingface.co/pipeline/feature-extraction/sentence-transformers/all-MiniLM-L6-v2";

//        public EmbeddingService(HttpClient http, IConfiguration config)
//        {
//            _http = http;
//            _apiKey = config["HuggingFaceAI:ApiKey"];
//        }

//        public async Task<float[]> GetVectorAsync(string text)
//        {
//            var request = new HttpRequestMessage(HttpMethod.Post, _modelUrl);
//            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
//            request.Content = new StringContent(JsonSerializer.Serialize(new { inputs = text }), Encoding.UTF8, "application/json");

//            var response = await _http.SendAsync(request);
//            var json = await response.Content.ReadAsStringAsync();

//            // Le modèle renvoie un tableau de float
//            return JsonSeriaslizer.Deserialize<float[]>(json);
//        }
//    }
//}
