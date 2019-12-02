using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;

namespace PetrolStation.Controllers
{
    public class StorageController : Controller
    {
        private readonly ApplicationContext _context;

        public StorageController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ViewProductList()
        {
            return View(await _context.Product.OrderBy(x => x.Name).ToListAsync());
        }

        // GET: Temp/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Temp/Create
        public IActionResult AddProductForm()
        {
            return View();
        }

        // POST: Temp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductForm([Bind("IdProduct,Name,QuantityInStorage,Price,PriceInPoints")] ProductString product)
        {
            //Debug.WriteLine(product.Price);
            try
            {
                decimal x = Convert.ToDecimal(product.Price);
            }
            catch(Exception)
            {
                return View(product);
            }
            Product p = new Product()
            {
                IdProduct = product.IdProduct,
                Name = product.Name,
                QuantityInStorage = product.QuantityInStorage,
                Price = Convert.ToDecimal(product.Price),
                PriceInPoints = product.PriceInPoints
            };
            //product.Price = Convert.ToDecimal(product.Price.ToString().Replace(".", ","));
            if (ModelState.IsValid)
            {
                _context.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewProductList");
                //return View(product);
            }
            return View(product);
        }

        // GET: Temp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Temp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProduct,Name,QuantityInStorage,Price,PriceInPoints")] Product product)
        {
            if (id != product.IdProduct)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.IdProduct))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Temp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Temp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.IdProduct == id);
        }
    }
}
