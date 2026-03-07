using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using System.Text.Json.Serialization;

namespace WebApplication1.Module
{
    public class CartItem
    {
            public int IdProduct { get; set; }
            public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        [JsonIgnore]
        public Product? product { get; set; }


    }
}
