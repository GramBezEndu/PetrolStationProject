using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze;

namespace PetrolStation.Controllers
{
    public class TransactionListController : Controller
    {
        private readonly ApplicationContext _context;
        private List<ExtendedTransactionModel> extendedTransactions = new List<ExtendedTransactionModel>();

        public TransactionListController(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Show all transactions
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var transactions = _context.Transaction.ToList();
            foreach(var t in transactions)
            {
                var actualExtendedTransaction = new ExtendedTransactionModel();
                //assign transaction
                actualExtendedTransaction.Transaction = t;
                //assign transaction type
                if (actualExtendedTransaction.Transaction is TransactionInvoice)
                    actualExtendedTransaction.IsInvoice = true;
                //assign loyality card
                actualExtendedTransaction.LoyalityCard = _context.LoyalityCard.Where(x => x.IdLoyalityCard == t.IdLoyalityCard).FirstOrDefault();
                //Inny klient moze byc na karcie lojalnościowej i na fakturze, ale musi być jakiś
                if (actualExtendedTransaction.Transaction is TransactionInvoice transactionInvoice)
                {
                    actualExtendedTransaction.Client = _context.Client.Where(x => x.IdClient == transactionInvoice.IdClient).ToList()[0];
                    if(actualExtendedTransaction.Client.Name != null)
                    {
                        actualExtendedTransaction.ClientString = actualExtendedTransaction.Client.Name;
                    }
                    else
                    {
                        actualExtendedTransaction.ClientString = actualExtendedTransaction.Client.FirstName + " "  + actualExtendedTransaction.Client.Surname;
                    }
                }
                //wlasciciel karty
                if (actualExtendedTransaction.LoyalityCard != null)
                {
                    actualExtendedTransaction.LoyalityCardOwner = _context.Client.Where(x => x.IdClient == actualExtendedTransaction.LoyalityCard.IdClient).FirstOrDefault();
                }
                ////assign client
                //if (actualExtendedTransaction.LoyalityCard != null)
                //{
                //    else
                //    {
                //        actualExtendedTransaction.LoyalityCardOwner = _context.Client.Where(x => x.IdClient == actualExtendedTransaction.LoyalityCard.IdClient).FirstOrDefault();
                //    }
                //}
                //assign car if there is an assigned client (if nie potrzebny chyba)
                var car = _context.Car.Where(x => x.Client == actualExtendedTransaction.Client).FirstOrDefault();
                actualExtendedTransaction.Car = car;
                //assign fueling list
                actualExtendedTransaction.FuelingList = _context.FuelingList.Where(x => x.Transaction == actualExtendedTransaction.Transaction).ToList();
                foreach(var fuelingItem in actualExtendedTransaction.FuelingList)
                {
                    //?
                    fuelingItem.Fueling = _context.Fueling.Where(x => x.IdFueling == fuelingItem.IdFueling).ToList()[0];
                    //?
                    fuelingItem.Fueling.Fuel = _context.Fuel.Where(x => x.IdFuel == fuelingItem.Fueling.IdFuel).ToList()[0];
                }
                //assign product list
                actualExtendedTransaction.PurchasedProducts = _context.ProductList.Where(x => x.Transaction == actualExtendedTransaction.Transaction).ToList();
                //dlaczego to konieczne?
                foreach(var purchasedProduct in actualExtendedTransaction.PurchasedProducts)
                {
                    //assign product
                    purchasedProduct.Product = _context.Product.Where(x => x.IdProduct == purchasedProduct.IdProduct).ToList()[0];
                }
                //Add extended transaction
                extendedTransactions.Add(actualExtendedTransaction);
            }
            return View(extendedTransactions);
        }
    }
}