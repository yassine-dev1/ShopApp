using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Category
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }

        // Liste des produits appartenant à cette catégorie
        public ICollection<Product> Products { get; set; }
    }
}
