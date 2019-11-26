using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze
{
    public class TransactionModel
    {
        public String boughtString { get; set; }
        public List<FuelingModel> purchasedFueling { get; set; } = new List<FuelingModel>();
        public decimal TransactionValue { get; set; }
        public bool IsInvoice { get; set; }
        public bool CardPayment { get; set; }
        public int IdLoyalityCard { get; set; }
        public Client client { get; set; } = new Client();
        public Car clientCar { get; set; } = new Car();
    }
}