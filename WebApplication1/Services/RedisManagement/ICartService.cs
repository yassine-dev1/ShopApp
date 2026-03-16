using WebApplication1.Models;

<<<<<<< HEAD
=======
// commentaire
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
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