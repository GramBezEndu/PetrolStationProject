using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class PumpTank
    {
        public GasPump GasPump { get; set; }
        [Column(Order = 1)]
        [ForeignKey("GasPump")]
        public int IdGasPump { get; set; }
        public FuelTank FuelTank { get; set; }
        [Column(Order = 2)]
        [ForeignKey("FuelTank")]
        public int IdFuelTank { get; set; }
    }
}
