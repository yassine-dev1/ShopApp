
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int QuantityInStock { get; set; }

        // 🔗 Clé étrangère vers Category
        [Required]
        public int CategoryId { get; set; }

        // Navigation
        public Category Category { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string ImageUrl { get; set; }
    }
}

