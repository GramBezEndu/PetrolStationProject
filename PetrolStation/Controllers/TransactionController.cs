using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.ExtensionMethods;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;
using System;
using System.Collections.Generic;
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
            ViewData["Karty"] = _context.LoyalityCard.ToList();
            ViewData["Produkty"] = _context.Product.ToList();
            ViewData["Klienci"] = _context.Client.ToList();
            ViewData["Samochody"] = _context.Car.ToList();
            TransactionModel transactionModel = new TransactionModel
            {
                IsInvoice = false,
                CardPayment = false,
            };

            //lista tankowań nierozliczonych
            var settledFuelings = _context.FuelingList.Select(fl => fl.IdFueling).ToList();
            var unsettledFuelings = _context.Fueling.Where(f => !settledFuelings.Contains(f.IdFueling)).OrderBy(f => f.IdGasPump).ToList();
            foreach (var item in unsettledFuelings)
            {
                var thisFuel = _context.Fuel.Find(item.IdFuel);
                FuelingModel fuelingModel = new FuelingModel(item);
                fuelingModel.fueling.Fuel = thisFuel;
                fuelingModel.VelueOfFueling = Convert.ToDecimal((Convert.ToDecimal(item.Quantity) * thisFuel.PriceForLiter).ToString("F"));
                transactionModel.purchasedFueling.Add(fuelingModel);

            }

            return View(transactionModel);
        }

        

        //POST AddTransaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransactionPOST(TransactionModel transactionModel)
        {
            //parsowanie stringa i stworzenie listy kupionych produktów
            List<ProductQuantity> purchasedProducts = new List<ProductQuantity>();
            if (transactionModel.boughtString != null)
            {
                string[] IdProductsAndQuantity = transactionModel.boughtString.Split(',', ';');
                for (int i = 0; i < IdProductsAndQuantity.Length - 1; i += 2)
                {
                    int key = Convert.ToInt32(IdProductsAndQuantity[i]);
                    var product = _context.Product.Find(key);
                    ProductQuantity productQuantity = new ProductQuantity
                    {
                        product = product,
                        Quantity = Convert.ToInt32(IdProductsAndQuantity[i + 1])
                    };
                    purchasedProducts.Add(productQuantity);
                }
            }

            Transaction transaction = new Transaction
            {
                Date = DateTime.Now
            };
            if (Convert.ToInt32(transactionModel.IdLoyalityCard) == 0)
                transaction.IdLoyalityCard = null; //jeśli w transakcji nie wpisaliśmy karty- nie ma powiązania
            else //do transakcji została dołączona karta
            {
                transaction.IdLoyalityCard = Convert.ToInt32(transactionModel.IdLoyalityCard);
                var pointsRequiredForPayment = Convert.ToDecimal(transactionModel.TransactionValue) * 100;
                var CardToAddPoints = await _context.LoyalityCard.FindAsync(transaction.IdLoyalityCard);
                if (transactionModel.CardPayment) //osoba chce zapłacić kartą
                {
                    if (CardToAddPoints.ActualPoints >= pointsRequiredForPayment) //jeśli na karcie jest wystarczająca ilość punktów
                    {
                        CardToAddPoints.ActualPoints -= Convert.ToInt32(pointsRequiredForPayment);
                        _context.Update(CardToAddPoints);
                    }
                    else //nie ma wystarczającej ilości punktów- powiadomienie o błędzie
                    {
                        ViewData["Karty"] = _context.LoyalityCard.ToList();
                        ViewData["Produkty"] = _context.Product.ToList();
                        ViewData["Klienci"] = _context.Client.ToList();
                        ViewData["Samochody"] = _context.Car.ToList();
                        ViewBag.Error = "This card doesn't have enough points to pay for this transaction";
                        return View("AddTransaction", transactionModel);
                    }
                }
                else //jeśli klient ma kartę, ale nie chce płacić punktami->dodajemy punkty za transakcję
                {
                    //dodanie punktów zależne od wartości transakcji. (!!!)dodanie wartości za tankowania w widoku
                    CardToAddPoints.ActualPoints += Convert.ToInt32(transactionModel.TransactionValue) / 10;
                    _context.Update(CardToAddPoints);
                }
            }
            Transaction thisTransaction;
            TransactionInvoice transactionInvoice = new TransactionInvoice();
            //jeśli klient chce fakture
            if (transactionModel.IsInvoice)
            {
                if (transactionModel.client.IdClient == 0) //nie ma klienta w systemie
                {
                    if(transactionModel.client.NIP==null && transactionModel.client.Surname == null) //kliknięto fakturę, ale nie wpisano wymaganych danych
                    {
                        //błąd!!!
                        ViewData["Karty"] = _context.LoyalityCard.ToList();
                        ViewData["Produkty"] = _context.Product.ToList();
                        ViewData["Klienci"] = _context.Client.ToList();
                        ViewData["Samochody"] = _context.Car.ToList();
                        ViewBag.Error = "Nie podano danych do faktury!!! Transakcja została wyzerowana. Spróbuj jeszcze raz";
                        transactionModel.IsInvoice = false;
                        return View("AddTransaction", transactionModel);//??
                    }
                    else //dodajemy klienta do bazy danych
                    {
                        Client client = new Client();
                        client = transactionModel.client;
                        _context.Add(client);
                        _context.SaveChanges();
                        var thisClient = _context.Client.Where(c=>c==client).ToList()[0];
                        transactionInvoice.Date = transaction.Date;
                        transactionInvoice.IdLoyalityCard = transaction.IdLoyalityCard;
                        transactionInvoice.IdClient = thisClient.IdClient;
                        transactionInvoice.IdCar = null;
                    }
                }
                else //klient jest w systemie
                {
                    transactionInvoice.Date = transaction.Date;
                    transactionInvoice.IdLoyalityCard = transaction.IdLoyalityCard;
                    transactionInvoice.IdClient = transactionModel.client.IdClient;
                    if (transactionModel.clientCar.IdCar == 0) //nie podano id samochodu
                    {
                        transactionInvoice.IdCar = null;
                    }
                    else //podano id samochodu
                    {
                        transactionInvoice.IdCar = transactionModel.clientCar.IdCar;
                    }
                }
                _context.Add(transactionInvoice);
                _context.SaveChanges();
                thisTransaction=_context.Transaction.Where(t => t == transactionInvoice).ToList()[0];
            }
            else //jeśli nie, to paragon
            {
                _context.Add(transaction);
                _context.SaveChanges();
                thisTransaction = _context.Transaction.Where(t => t == transaction).ToList()[0];
            }

            //dodanie rekordu do tabeli "Transactions" oraz zapisanie zmian

            //dodajemy powiązania do tabeli "ListaTowarów"
            foreach (var product in purchasedProducts)
            {
                ProductList productList = new ProductList
                {
                    IdTransaction = thisTransaction.IdTransaction,
                    IdProduct = product.product.IdProduct,
                    Quantity = product.Quantity
                };
                Product productToUpdateQuantityStorage = product.product;
                //odejmujemy ze stanu, ilosć sprzedanego towaru
                productToUpdateQuantityStorage.QuantityInStorage -= productList.Quantity;
                _context.Update(productToUpdateQuantityStorage);
                _context.Add(productList);
            }
           // _context.SaveChanges();
            //obsługa sprzedanego paliwa

            foreach (var item in transactionModel.purchasedFueling)
            {
                if (item.IsChecked)
                {
                    FuelingList fuelingList = new FuelingList
                    {
                        IdTransaction = thisTransaction.IdTransaction,
                        IdFueling = item.fueling.IdFueling,
                    };
                    _context.Add(fuelingList);
                    //??? Będziemy odejmować sprzedane paliwo ze zbiorników???
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}