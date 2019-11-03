using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class FuelingList
    {
        public Fueling Fueling { get; set; }
        [Key]
        [ForeignKey("Fueling")]
        public int IdFueling { get; set; }
        public Transaction Transaction { get; set; }
        [ForeignKey("Transaction")]
        public int IdTransaction { get; set; }
    }
}
