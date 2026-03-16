     namespace WebApplication1.Services.AI.Embedding
    {
        public interface IEmbeddingService
        {
            Task<float[]> CreateEmbeddingAsync(string query);
            Task<float[]> GenerateEmbeddingAsync(string text);
        }
    }

