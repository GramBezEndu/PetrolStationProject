using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

    }
}
