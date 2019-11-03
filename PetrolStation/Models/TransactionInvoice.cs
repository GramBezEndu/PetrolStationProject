using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class TransactionInvoice : Transaction
    {
        public Client Client { get; set; }
        [ForeignKey("Client")]
        public int IdClient { get; set; }
        public Car Car { get; set; }
        [ForeignKey("Car")]
        public int IdCar { get; set; }
    }
}
