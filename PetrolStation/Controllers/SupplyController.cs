using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;

namespace PetrolStation.Controllers
{
    public class SupplyController : Controller
    {
        private readonly ApplicationContext _context;

        public SupplyController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Supply
        public async Task<IActionResult> AddSupply()
        {
            ViewData["Produkty"] = await _context.Product.ToListAsync();

            SupplyString supply = new SupplyString();
            return View(supply);
        }
        [HttpPost]
        public async Task<IActionResult> AddSupply(SupplyString supply)
        {
            if (supply.supplyString != null)
            {
                string[] IdProductsAndQuantity = supply.supplyString.Split(' ');
                for (int i = 0; i < IdProductsAndQuantity.Length - 1; i += 2)
                {
                    int key = Convert.ToInt32(IdProductsAndQuantity[i]);
                    var product = _context.Product.Find(key);
                    product.QuantityInStorage =Convert.ToInt32(IdProductsAndQuantity[i + 1]);
                    _context.Update(product);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
