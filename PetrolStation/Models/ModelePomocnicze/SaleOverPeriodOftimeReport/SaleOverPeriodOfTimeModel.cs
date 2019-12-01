using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze.SaleOverPeriodOfTimeReport
{
    public class SaleOverPeriodOfTimeModel
    {
        public DateTime PoczatekPrzedzialu { get; set; }
        public DateTime KoniecPrzedzialu { get; set; }
        public List<ProductNameQuantity> soldProducts { get; set; } = new List<ProductNameQuantity>();
        public int Order { get; set; }
    }
}
