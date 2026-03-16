namespace WebApplication1.Services.AI.Retrieval
{
    using AI.Embedding;
    using AI.VectorStore;

    public class ProductRetrievalService : IRetrievalService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorStore _vectorStore;

        public ProductRetrievalService(
            IEmbeddingService embeddingService,
            IVectorStore vectorStore)
        {
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
        }

        public async Task<List<RetrievedDocument>> RetrieveAsync(
            string query,
            int topK = 15)
        {
            // 1️⃣ créer l'embedding de la question
            var embedding = await _embeddingService.GenerateEmbeddingAsync(query);

            // 2️⃣ rechercher dans la base vectorielle
            var results = await _vectorStore.SearchAsync(embedding, topK, query);

            //Console.WriteLine("************ProductRetrievalService***** :");
            //Console.WriteLine("query :" + query);
            //Console.WriteLine("embedding query size:" + embedding.Count());
            //Console.WriteLine("documents retreive size :" + results.Count());

            // 3️⃣ transformer les résultats
            var documents = results
            .OrderBy(r => r.Score)
            .Select(r => new RetrievedDocument
            {
                Id = r.Id,
                Content = r.Content,
                Score = r.Score
            }).ToList();

            return documents;
        }
    }
}
