using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class Fueling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFueling { get; set; }
        [Required]
        public float Quantity { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public Fuel Fuel { get; set; }
        [ForeignKey("Fuel")]
        public int IdFuel { get; set; }
        public GasPump GasPump { get; set; }
        [ForeignKey("GasPump")]
        public int IdGasPump { get; set; }
    }
}
