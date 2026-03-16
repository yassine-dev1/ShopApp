using WebApplication1.Models;

namespace WebApplication1.Services.RedisManagement
{
    public interface ICartService
    {
        Task AddToCartAsync(int productId, int quantity);
        Task<Dictionary<int, CartItemCache>> GetCartAsync();
        Task<int> GetTotalQuantityAsync();
        Task ClearAsync();
        Task RemoveItemAsync(int productId);
        Task<decimal> GetTotalAsync();
         Task UpdateQuantityAsync(int productId, int quantity);
    }
}