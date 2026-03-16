namespace WebApplication1.Services.AI.InitializationDBV
{
    using WebApplication1.Models;
    public interface IVectorDbInitializer
    {
        Task InitializeAsync();
        Task AddChunksProductInDBV(Product product);
    }
}
