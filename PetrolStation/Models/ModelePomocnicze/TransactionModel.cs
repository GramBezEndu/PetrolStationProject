using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze
{
    public class TransactionModel
    {
        public List<ProductQuantity> purchasedProducts { get; set; } = new List<ProductQuantity>();
        public string NamePurchasedProduct { get; set; }
        public int QuantityPurchasedProduct { get; set; }
        public decimal TransactionValue { get; set; }
        public bool IsInvoice { get; set; }
        public int IdLoyalityCard { get; set; }
        public Client client { get; set; } = new Client();
    }
}
