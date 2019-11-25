using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.ExtensionMethods;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            transactionModel.QuantityPurchasedProduct = 1;
            transactionModel.IsInvoice = false;
            transactionModel.CardPayment = false;
            transactionModel.TransactionValue = 0;

            //lista tankowań nierozliczonych
            var settledFuelings = _context.FuelingList.Select(fl => fl.IdFueling).ToList();
            var unsettledFuelings = _context.Fueling.Where(f => !settledFuelings.Contains(f.IdFueling)).OrderBy(f=>f.IdGasPump).ToList();
            foreach(var item in unsettledFuelings)
            {
                var thisFuel = _context.Fuel.Find(item.IdFuel);
                FuelingModel fuelingModel = new FuelingModel(item);
                fuelingModel.fueling.Fuel = thisFuel;
                fuelingModel.VelueOfFueling = Convert.ToDecimal((Convert.ToDecimal(item.Quantity) * thisFuel.PriceForLiter).ToString("F"));
                transactionModel.purchasedFueling.Add(fuelingModel);
                
            }

            return View(transactionModel);
        }

        [HttpPost]
        public IActionResult AddProduct(TransactionModel transactionModel)
        {
            var szukanyProdukt = _context.Product.Where(p => p.Name == transactionModel.NamePurchasedProduct).ToList();
            if (szukanyProdukt[0].QuantityInStorage < transactionModel.QuantityPurchasedProduct)
            {
                ViewData["Produkty"] = new SelectList(_context.Product, "Name", "Name");
                ViewData["Error"] = "Nie można dodać " + transactionModel.QuantityPurchasedProduct + " " + transactionModel.NamePurchasedProduct + " do koszyka, ponieważ w magazynie zostało jedynie " + szukanyProdukt[0].QuantityInStorage + " sztuk!";
                return View("AddTransaction", transactionModel);
            }
            ProductQuantity productQuantity = new ProductQuantity();
            productQuantity.product = szukanyProdukt[0];
            productQuantity.Quantity = transactionModel.QuantityPurchasedProduct;
            transactionModel.purchasedProducts.Add(productQuantity);
            //wyzerowanie
            transactionModel.TransactionValue = 0;
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
        public async Task<IActionResult> AddTransactionPOST(TransactionModel transactionModel)
        {
            Transaction transaction = new Transaction
            {
                Date = DateTime.Now
            };
            if (transactionModel.IdLoyalityCard == 0)
                transaction.IdLoyalityCard = null; //jeśli w transakcji nie wpisaliśmy karty- nie ma powiązania
            else //do transakcji została dołączona karta
            {
                transaction.IdLoyalityCard = transactionModel.IdLoyalityCard;
                var pointsRequiredForPayment = transactionModel.TransactionValue*100;
                var CardToAddPoints = await _context.LoyalityCard.FindAsync(transaction.IdLoyalityCard);
                if (transactionModel.CardPayment)
                {
                    if (CardToAddPoints.ActualPoints >= pointsRequiredForPayment)
                    {
                        CardToAddPoints.ActualPoints -= Convert.ToInt32(pointsRequiredForPayment);
                        _context.Update(CardToAddPoints);
                    }
                    else
                    {
                        ViewData["Produkty"] = new SelectList(_context.Product, "Name", "Name");
                        ViewData["Error"] = "This card doesn't have enough points to pay for this transaction";
                        return View("AddTransaction", transactionModel);
                    }
                }
                //dodanie punktów zależne od wartości transakcji. (!!!)dodanie wartości za tankowania w widoku
                CardToAddPoints.ActualPoints += Convert.ToInt32(transactionModel.TransactionValue) / 10;
                _context.Update(CardToAddPoints);
            }
            //dodanie rekordu do tabeli "Transactions" oraz zapisanie zmian
            _context.Add(transaction);
            _context.SaveChanges();
            var thisTransaction = _context.Transaction.Where(t => t == transaction).ToList(); //pobranie utworzonej transakcji

            //dodajemy powiązania do tabeli "ListaTowarów"
            foreach (var product in transactionModel.purchasedProducts)
            {
                ProductList productList = new ProductList
                {
                    IdTransaction = thisTransaction[0].IdTransaction,
                    IdProduct = product.product.IdProduct,
                    Quantity = product.Quantity
                };
                Product productToUpdateQuantityStorage = product.product;
                //odejmujemy ze stanu, ilosć sprzedanego towaru
                productToUpdateQuantityStorage.QuantityInStorage -= productList.Quantity;
                _context.Update(productToUpdateQuantityStorage);
                _context.Add(productList);
            }
            _context.SaveChanges();
            //obsługa sprzedanego paliwa

            foreach(var item in transactionModel.purchasedFueling)
            {
                if (item.IsChecked == true)
                {
                    FuelingList fuelingList = new FuelingList
                    {
                        IdTransaction = thisTransaction[0].IdTransaction,
                        IdFueling = item.fueling.IdFueling,
                    };
                    _context.Add(fuelingList);
                }
            }
            _context.SaveChanges();

            if (transactionModel.IsInvoice) //faktura
            {
               
                
            }

            //_context.Add(transaction);
            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));

            return View("AddTransaction", transactionModel);
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
            //ViewData["IdLoyalityCard"] = new SelectList(_context.LoyalityCard, "IdLoyalityCard", "IdLoyalityCard", transaction.IdLoyalityCard);
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
