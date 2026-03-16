namespace WebApplication1.Services.AI.SynchronizationDB
{
    using WebApplication1.Models;
    using AI.InitializationDBV;
    using AI.VectorStore;
    public class AIIndexingService : IAIIndexingService
    {
        private  readonly IVectorDbInitializer _initializerService;
        private readonly IVectorStore _vectorStore;
        public AIIndexingService(
                                 IVectorDbInitializer VI ,
                                 IVectorStore vectorStore
                                ){
            _initializerService = VI;
            _vectorStore = vectorStore;
        }

        public  async Task IndexProductAsync(Product product)
        {
             await _initializerService.AddChunksProductInDBV(product);
        }

        public async Task ReindexProductAsync(Product product) {

            await DeleteProductAsync(product.Id);

            await IndexProductAsync(product);
        }

        public async Task DeleteProductAsync(int productId)
        {
            var keys = await _vectorStore.GetChunkKeysByProductIdAsync(productId);

            if (keys == null || !keys.Any())
                return;

            await _vectorStore.DeleteManyAsync(keys);

        }
    }
}
