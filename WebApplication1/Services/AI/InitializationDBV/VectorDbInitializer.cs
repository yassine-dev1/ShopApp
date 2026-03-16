namespace WebApplication1.Services.AI.InitializationDBV
{
    using AI.Embedding;
    using AI.VectorStore;
    using WebApplication1.Services.ServiceDAO;
    using WebApplication1.Models;

    public class VectorDbInitializer : IVectorDbInitializer
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorStore _vectorStore;
        private readonly ProduitDAO _produitDao;

        public VectorDbInitializer(
            IEmbeddingService embeddingService,
            IVectorStore vectorStore,
            ProduitDAO produitDao)
        {
            _embeddingService = embeddingService;
            _vectorStore = vectorStore;
            _produitDao = produitDao;
        }

        public async Task InitializeAsync()
        {
            // si Redis contient déjà des vecteurs on ne réinitialise pas
            if (!await _vectorStore.IsEmptyAsync())
                return;

            var products = await _produitDao.GetAllProduitsAsync();

            foreach (var product in products)
            {
                _ = AddChunksProductInDBV(product);
            }
        }


        // ----------------- store chunks product in DBV -------------------//
        /**
         * @bref store chunks of specifique product in DBV with use startegy chunks << produit = three chunks(embedding) >> 
         * @objectif improve Qualite RAG systeme specifique in  search precision 
         * @param product product objet
         **/
        public  async Task AddChunksProductInDBV(Product product)
        {
            var chunks = BuildProductChunks(product);

            int chunkIndex = 0;

            foreach (var chunk in chunks)
            {
                var embedding = await _embeddingService.CreateEmbeddingAsync(chunk);

                await _vectorStore.StoreAsync(
                    $"{product.Id}_chunk_{chunkIndex}",
                    embedding,
                    chunk,
                    new Dictionary<string, string>
                    {
                        ["title"] = product.Name,
                        ["category"] = product.Category?.Name ?? "unknown",
                        ["productId"] = product.Id.ToString()
                    });

                chunkIndex++;
            }
        }
       

        // ----------- PRODUCT CHUNKING STRATEGY -----------

        private List<string> BuildProductChunks(Product product)
        {
            var chunks = new List<string>();

            // chunk 1 : résumé produit
            chunks.Add($"""
            Product:  {product.Name}
            Category: {product.Category?.Name ?? "Unknown"}
            Price:    {product.Price}
            Status:   {(product.IsActive ? "Active" : "Inactive")}
            """);

            // chunk 2 : description
            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                chunks.Add($"""
                Product: {product.Name}
                Description:
                {product.Description}
                """);
            }

            // chunk 3 : stock
            chunks.Add($"""
            Product: {product.Name}
            Stock information:
            Quantity in stock: {product.QuantityInStock}
            """);

            return chunks;
        }
    }
}