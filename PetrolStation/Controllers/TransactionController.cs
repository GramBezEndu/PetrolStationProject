using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.ExtensionMethods;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;

namespace PetrolStation.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationContext _context;

        public TransactionController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.Transaction.Include(t => t.LoyalityCard);
            return View(await applicationContext.ToListAsync());
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.LoyalityCard)
                .FirstOrDefaultAsync(m => m.IdTransaction == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transaction/Create
        public IActionResult AddTransaction()
        {
            ViewData["IdLoyalityCard"] = new SelectList(_context.LoyalityCard, "IdLoyalityCard", "IdLoyalityCard");
            ViewData["Produkty"] = new SelectList(_context.Product, "Name", "Name");
            TransactionModel transactionModel = new TransactionModel();
            transactionModel.TransactionValue = 0;
            return View(transactionModel);
        }

        
        public IActionResult AddProduct(TransactionModel transactionModel)
        {
            var szukanyProdukt = _context.Product.Where(p => p.Name == transactionModel.NamePurchasedProduct).ToList();
            ProductQuantity productQuantity = new ProductQuantity();
            productQuantity.product = szukanyProdukt[0];
            productQuantity.Quantity = transactionModel.QuantityPurchasedProduct;
            transactionModel.purchasedProducts.Add(productQuantity);
            foreach (var item in transactionModel.purchasedProducts)
                transactionModel.TransactionValue += (item.Quantity * item.product.Price);
            transactionModel.QuantityPurchasedProduct = 1;
            TempData.Put("key", transactionModel);
            return RedirectToAction("AddProductWyswietl");
        }

        public IActionResult AddProductWyswietl()
        {
            var transactionModel = TempData.Get<TransactionModel>("key");
            ViewData["Produkty"] = new SelectList(_context.Product, "Name", "Name");
            return View("AddTransaction", transactionModel);
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransationPOST([Bind("IdTransaction,IdLoyalityCard,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdLoyalityCard"] = new SelectList(_context.LoyalityCard, "IdLoyalityCard", "IdLoyalityCard", transaction.IdLoyalityCard);
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["IdLoyalityCard"] = new SelectList(_context.LoyalityCard, "IdLoyalityCard", "IdLoyalityCard", transaction.IdLoyalityCard);
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTransaction,IdLoyalityCard,Date")] Transaction transaction)
        {
            if (id != transaction.IdTransaction)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.IdTransaction))
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
            ViewData["IdLoyalityCard"] = new SelectList(_context.LoyalityCard, "IdLoyalityCard", "IdLoyalityCard", transaction.IdLoyalityCard);
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .Include(t => t.LoyalityCard)
                .FirstOrDefaultAsync(m => m.IdTransaction == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transaction.Any(e => e.IdTransaction == id);
        }
    }
}
