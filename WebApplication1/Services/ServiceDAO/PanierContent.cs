using WebApplication1.Services.RedisManagement;
using WebApplication1.Models;
using WebApplication1.Module;

namespace WebApplication1.Services.ServiceDAO
{
    public class PanierContent
    {
        private CartRedisService cartRedis;

        public PanierContent(CartRedisService cartRedisService)
        {
            cartRedis = cartRedisService;
        }
        public Task<Dictionary<int, CartItemCache>> GetPanierContentAsync()
        {
            return cartRedis.GetCartAsync();
        }
    }
}
