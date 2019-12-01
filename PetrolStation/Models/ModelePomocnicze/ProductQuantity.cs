using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze
{
    public class ProductQuantity
    {
        public Product product { get; set; } = new Product();
        public int Quantity { get; set; }
    }
}
