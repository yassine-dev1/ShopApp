namespace WebApplication1.Models
{
    public class CartItemCache
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal PriceAtAddTime { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
