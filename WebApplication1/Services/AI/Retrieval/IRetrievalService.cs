namespace WebApplication1.Services.AI.Retrieval
{
    public interface IRetrievalService
    {
        Task<List<RetrievedDocument>> RetrieveAsync(string query, int topK = 5);
    }

    public class RetrievedDocument
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public double Score { get; set; }
    }
}
