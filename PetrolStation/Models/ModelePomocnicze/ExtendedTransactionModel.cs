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
        public string ClientString
        {
            get
            {
                if(Client != null)
                {
                    if (Client.Name != null)
                        return Client.Name;
                    else
                        return String.Format("{0} {1}", Client.FirstName, Client.Surname);
                }
                else if(LoyalityCardOwner != null)
                {
                    if (LoyalityCardOwner.Name != null)
                        return LoyalityCardOwner.Name;
                    else
                        return String.Format("{0} {1}", LoyalityCardOwner.FirstName, LoyalityCardOwner.Surname);
                }
                else
                    return "-";
            }
        }
        public string TransactionType
        {
            get
            {
                if (IsInvoice)
                    return "Invoice";
                else
                    return "Transaction";
            }
        }
    }
}
