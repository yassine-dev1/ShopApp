
using WebApplication1.Services.AI.Retrieval;

namespace WebApplication1.Services.AI.Reranking
{
        public interface IRerankerService
        {
            Task<List<RetrievedDocument>> RerankAsync(
                string query,
                List<RetrievedDocument> documents,
                int topK = 5);
        }
    
}
