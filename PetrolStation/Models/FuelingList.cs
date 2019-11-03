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
        public Transaction Transaction { get; set; }
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Transaction")]
        public int IdTransaction { get; set; }
        public Fueling Fueling { get; set; }
        [Key]
        [Column(Order = 2)]
        [ForeignKey("Fueling")]
        public int IdFueling { get; set; }
    }
}
