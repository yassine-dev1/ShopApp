using WebApplication1.Services.AI.Retrieval;

namespace WebApplication1.Services.AI.Reranking
{
    public class RerankerService : IRerankerService
    {
        public Task<List<RetrievedDocument>> RerankAsync(
            string query,
            List<RetrievedDocument> documents,
            int topK = 5)
        {
            var queryWords = query.ToLower().Split(" ");

            var ranked = documents
                .Select(d =>
                {
                    int keywordScore = queryWords.Count(w => d.Content.ToLower().Contains(w));

                    double finalScore = d.Score - keywordScore;

                    return new RetrievedDocument
                    {
                        Id = d.Id,
                        Content = d.Content,
                        Score = finalScore
                    };
                })
                .OrderBy(d => d.Score)
                .Take(topK)
                .ToList();

            return Task.FromResult(ranked);
        }
    }
}
