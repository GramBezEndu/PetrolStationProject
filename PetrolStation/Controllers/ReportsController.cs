using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetrolStation.ExtensionMethods;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze.SaleOverPeriodOfTimeReport;

namespace PetrolStation.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationContext _context;

        public ReportsController(ApplicationContext context)
        {
            _context = context;
        }


        //4.3. Całkowita sprzedaż z wybranego okresu (dzień, miesiąc, rok) 
        public IActionResult SaleOverPeriodOfTime()
        {
            SaleOverPeriodOfTimeModel saleOverPeriodOfTime = new SaleOverPeriodOfTimeModel();
            List<int> IdProductAddedToModel = new List<int>();
            var allProductNames = _context.Product.Select(p => new { p.IdProduct, p.Name }).ToList();
            var allSalesProduct = _context.ProductList.ToList();
            for(int i = 0; i < allSalesProduct.Count; i++)
            {
                if (IdProductAddedToModel.Contains(allSalesProduct[i].IdProduct))
                {
                    foreach(var item in saleOverPeriodOfTime.soldProducts)
                    {
                        if (item.Id == allSalesProduct[i].IdProduct)
                        {
                            item.SoldQuantity += allSalesProduct[i].Quantity;
                        }
                    }
                }
                else
                {
                    ProductNameQuantity productNameQuantity = new ProductNameQuantity
                    {
                        Id = allSalesProduct[i].IdProduct,
                        Name = allProductNames.Where(pn => pn.IdProduct == allSalesProduct[i].IdProduct).Select(pn => pn.Name).ToList()[0],
                        SoldQuantity = allSalesProduct[i].Quantity
                    };
                    saleOverPeriodOfTime.soldProducts.Add(productNameQuantity);
                    IdProductAddedToModel.Add(allSalesProduct[i].IdProduct);
                }
            }
            saleOverPeriodOfTime.soldProducts=saleOverPeriodOfTime.soldProducts.OrderBy(sp => sp.Name).ToList();
            saleOverPeriodOfTime.PoczatekPrzedzialu = DateTime.Today;
            saleOverPeriodOfTime.KoniecPrzedzialu = DateTime.Today;

            return View(saleOverPeriodOfTime);
        }
        [HttpPost]
        public IActionResult SaleOverPeriodOfTime(SaleOverPeriodOfTimeModel saleOverPeriodOfTimeModel)
        {
            TempData.Put<SaleOverPeriodOfTimeModel>("Model", saleOverPeriodOfTimeModel);
            return RedirectToAction("GenerateSaleOverPeriodOfTime");
        }

        public IActionResult GenerateSaleOverPeriodOfTime()
        {
            List<int> IdProductAddedToModel = new List<int>();
            var directive = TempData.Get<SaleOverPeriodOfTimeModel>("Model");
            //nazwy wszyskich produktów
            var allProductNames = _context.Product.Select(p => new { p.IdProduct, p.Name }).ToList();
            //transakcje których data mieści się w przedziale
            var transactionsInRange = _context.Transaction
                .Where(t => t.Date > directive.PoczatekPrzedzialu && t.Date < directive.KoniecPrzedzialu)
                .Select(t => t.IdTransaction)
                .ToList();
            //produkty kupione w powyższych transakcjach
            var productsInTransaction = _context.ProductList
                .Where(pl => transactionsInRange.Contains(pl.IdTransaction))
                .ToList();
            SaleOverPeriodOfTimeModel sale = new SaleOverPeriodOfTimeModel
            {
                KoniecPrzedzialu = directive.KoniecPrzedzialu,
                PoczatekPrzedzialu = directive.PoczatekPrzedzialu,
                Order = directive.Order
            };
            foreach(var product in productsInTransaction)
            {
                if (IdProductAddedToModel.Contains(product.IdProduct))
                {
                    sale.soldProducts.Where(sp => sp.Id == product.IdProduct)
                        .ToList()[0].SoldQuantity += product.Quantity;
                }
                else
                {
                    ProductNameQuantity productNameQuantity = new ProductNameQuantity
                    {
                        Id = product.IdProduct,
                        Name = allProductNames.Where(apn => apn.IdProduct == product.IdProduct)
                        .ToList()[0].Name,
                        SoldQuantity = product.Quantity
                    };
                    sale.soldProducts.Add(productNameQuantity);
                    IdProductAddedToModel.Add(product.IdProduct);
                }
            }

            switch (directive.Order)
            {
                case 1:
                    sale.soldProducts = sale.soldProducts.OrderBy(sp => sp.Id).ToList();
                    break;
                case 2:
                    sale.soldProducts = sale.soldProducts.OrderBy(sp => sp.Name).ToList();
                    break;
                case 3:
                    sale.soldProducts = sale.soldProducts.OrderBy(sp => sp.SoldQuantity).ToList();
                    break;
            }

            return View("SaleOverPeriodOfTime",sale);
        }
    }
}
