using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class FuelTank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFuelTank { get; set; }
        [Required]
        public float Capacity { get; set; }
        [Required]
        public float ActualQuantity { get; set; }
        public Fuel Fuel { get; set; }
        [ForeignKey("Fuel")]
        public int IdFuel { get; set; }
    }
}
