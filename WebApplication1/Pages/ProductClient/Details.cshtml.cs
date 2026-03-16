using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Pages.ProductClient
{
    public class DetailsModel : PageModel
    {
        private readonly WebApplication1.Data.WebApplication1Context _context;

        public DetailsModel(WebApplication1.Data.WebApplication1Context context)
        {
            _context = context;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

<<<<<<< HEAD
            var product = await _context.Product.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
=======
            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
>>>>>>> b218cbdaaaed9a106850a8e85846553864058608

            if (product is not null)
            {
                Product = product;

                return Page();
            }

            return NotFound();
        }
    }
}
