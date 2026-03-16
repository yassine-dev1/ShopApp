using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
<<<<<<< HEAD
=======
// commnentairegit status
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608

namespace WebApplication1.Services.ServiceDAO
{
    public class ProduitDAO
    {
        private readonly WebApplication1Context _context;


       public  ProduitDAO(WebApplication1Context context)
        {
            _context = context;
        }


        public async  Task<List<Product>> GetAllProduitsAsync()
        {
<<<<<<< HEAD
            return await _context.Product.Include(p => p.Category).ToListAsync();
=======
            return await _context.Product.ToListAsync();
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
        }

        public async Task<Product?> GetProduitByIdAsync(int id)
        {
            return await _context.Product.FindAsync(id);
        }

        public async Task AddProduitAsync(Product produit)
        {
            _context.Product.Add(produit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduitAsync(Product produit)
        {
            _context.Product.Update(produit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduitAsync(int id)
        {
            var produit = await _context.Product.FindAsync(id);
            if (produit != null)
            {
                _context.Product.Remove(produit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Product>> SearchProduitsAsync(string searchTerm)
        {
            return await _context.Product
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
<<<<<<< HEAD
                .Include(p => p.Category)
=======
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608
                .ToListAsync();
        }

        public async Task<List<Product>> GetProduitsByCategoryAsync(string category)
        {
            return await _context.Product
                .Where(p => p.Category.Name == category)
                .ToListAsync();
        }

    }
}
