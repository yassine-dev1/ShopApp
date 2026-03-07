using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context (DbContextOptions<WebApplication1Context> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Force le nom de la table à être "Category"
            modelBuilder.Entity<Category>().ToTable("Category");

            // Force le nom de la table à être "Product"
            modelBuilder.Entity<Product>().ToTable("Product");

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<WebApplication1.Models.Product> Product { get; set; } = default!;
        public DbSet<WebApplication1.Models.Category> Category { get; set; } = default!;
    }
}
