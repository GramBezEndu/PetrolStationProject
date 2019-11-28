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
            
            return RedirectToAction("Index", "Home");
        }
    }
}
