using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Configuration;
using System.Numerics;
using WebApplication1.Models;
using WebApplication1.Services.RedisManagement;

namespace WebApplication1.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;

        public IndexModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public Dictionary<int, CartItemCache> Cart { get; set; } = new();
        public decimal Total { get; set; }

        public async Task OnGetAsync()
        {
            Cart = await _cartService.GetCartAsync();
            Total = await _cartService.GetTotalAsync();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            await _cartService.RemoveItemAsync(id);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostClearAsync()
        {
            await _cartService.ClearAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPayAsync()
        {
            await _cartService.ClearAsync();
            TempData["Success"] = "Paiement confirmé avec succès ✅";
            return RedirectToPage();
        }

        public async  Task<IActionResult> OnPostUpdateQuantityAsync(int id , int quantity)
        {
            await _cartService.UpdateQuantityAsync(id, quantity);
            return RedirectToPage();

        }
    }
}
