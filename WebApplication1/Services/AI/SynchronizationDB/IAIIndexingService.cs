namespace WebApplication1.Services.AI.SynchronizationDB
{
    using WebApplication1.Models;

    public interface IAIIndexingService
    {
        Task IndexProductAsync(Product product);

        Task ReindexProductAsync(Product product);

        Task DeleteProductAsync(int productId);
    }
}
