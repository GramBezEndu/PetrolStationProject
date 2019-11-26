using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze
{
    public class ExtendedTransactionModel
    {
        public Transaction Transaction { get; set; }
        public bool IsInvoice { get; set; }
        public LoyalityCard LoyalityCard { get; set; }
        public Client Client { get; set; }
        public Client LoyalityCardOwner { get; set; }
        public Car Car { get; set; }
        public List<ProductList> PurchasedProducts { get; set; } = new List<ProductList>();
        public List<FuelingList> FuelingList { get; set; } = new List<FuelingList>();

    }
}
