using StackExchange.Redis;

namespace WebApplication1.Services.AI.VectorStore
{
    public interface IVectorStore
    {
        Task StoreAsync(string id, float[] embedding, string content, Dictionary<string, string>? metadata = null);

        Task<List<VectorSearchResult>> SearchAsync(float[] embedding, int topK = 5, string query = "");

        Task DeleteAsync(string id);

        Task<bool> IsEmptyAsync();

        Task<bool> ExistsKeyAsync(string id);
        
        Task<List<string>> GetChunkKeysByProductIdAsync(int productId);

        Task DeleteManyAsync(List<string> keys);

    }

    public class VectorSearchResult
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public double Score { get; set; }
    }

}
