
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebApplication1.Data;
using WebApplication1.Models;
<<<<<<< HEAD
=======
// commentaire
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608

namespace WebApplication1.Services.RedisManagement
{
    public class CartRedisService : ICartService
    {
        private readonly IDistributedCache _redis;
        private readonly WebApplication1Context _context;
        private readonly IHttpContextAccessor _http;

        public CartRedisService(
            IDistributedCache redis,
            WebApplication1Context context,
            IHttpContextAccessor http)
        {
            _redis = redis;
            _context = context;
            _http = http;
        }

        // 🔑 Clé Redis
        private string CartKey
        {
            get
            {
                var context = _http.HttpContext!;

                //if (context.User.Identity?.IsAuthenticated == true)
                //    return $"CART:{context.User.Identity.Name}";


                // genérer une Id et stocker dans une cookie c'est la cookie CART_ID n'existe pas   
                if (!context.Request.Cookies.ContainsKey("CART_ID"))
                {
                    var cartId = Guid.NewGuid().ToString();
                    context.Response.Cookies.Append("CART_ID", cartId);
                    return $"CART:{cartId}";
                }

                // récupérer Id a partir du cookie
                return $"CART:{context.Request.Cookies["CART_ID"]}";
            }
        }


        // fonction  pour récupérer le panier sous forme dictionnaire <key , value(object)>
        public async Task<Dictionary<int, CartItemCache>> GetCartAsync()
        {
            // recupérer le cache du panier
            var json = await _redis.GetStringAsync(CartKey);

            return json == null
                ? new Dictionary<int, CartItemCache>()
                : JsonSerializer.Deserialize<Dictionary<int, CartItemCache>>(json)!;
        }

        public async Task<int> GetTotalQuantityAsync()
        {
            var cart = await GetCartAsync();
            return cart.Count();
        }


        // fonction pour ajouter au panier
        public async Task AddToCartAsync(int productId, int quantity)
        {
            // recupérer info sur le produit
            var product = await _context.Product.FindAsync(productId);
            if (product == null) return;

            var cart = await GetCartAsync();

            // si le produit existe déja dans le panier
            if (cart.ContainsKey(productId))
            {
                cart[productId].Quantity += quantity;  // modifier seulemnt la quantité
            }
            else   
            {
                /*sinon crée une nouvelle object (CartItemCache)  sous forme <key , value> 
                 * but => recherche constante O(1)
                */
                cart[productId] = new CartItemCache
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    PriceAtAddTime = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = quantity
                };
            }

            await SaveCartAsync(cart);
        }


        // nettoyer le panier
        public async Task ClearAsync()
        {
            await _redis.RemoveAsync(CartKey);
        }


        // sauvegarder le panier
        private async Task SaveCartAsync(Dictionary<int, CartItemCache> cart)
        {
            var json = JsonSerializer.Serialize(cart);

            await _redis.SetStringAsync(
                CartKey,
                json,
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(2)
                });
        }


        // function pour modifié la quantité d'une produit 
        public async Task UpdateQuantityAsync(int productId , int quantity)
        {
            var cart = await GetCartAsync();
            if (cart.ContainsKey(productId))
            {
                //Cart.TryGetValue(id, out var item);
                // recovere actuel quantity
                var ancienneQuantity = cart[productId].Quantity;
                // check ancienne quantity with new quantity than differente and great a 0
                if (ancienneQuantity != quantity && quantity > 0)
                {
                    cart[productId].Quantity = quantity;
                    SaveCartAsync(cart);
                }
                
            }

        }

        // function pour supprimé une produit dans le panier
        public async Task RemoveItemAsync(int productId)
        {
            var cart = await GetCartAsync();

            // vérifié si le produit existe dans le panier
            if (cart.ContainsKey(productId))
            {
                cart.Remove(productId);
                await SaveCartAsync(cart);
            }
        }


        // function pour calculé le total des achats 
        public async Task<decimal> GetTotalAsync()
        {
            var cart = await GetCartAsync();
            return cart.Sum(x => x.Value.PriceAtAddTime * x.Value.Quantity);
        }
    }
}

