using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCar { get; set; }
        public Client Client { get; set; }
        [ForeignKey("Client")]
        public int IdClient { get; set; }
        [Required]
        public string NumberPlate { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
    }
}
