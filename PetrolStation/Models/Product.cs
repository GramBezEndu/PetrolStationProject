using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdProduct { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int QuantityInStorage { get; set; }
        [DataType(DataType.Currency)]
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int PriceInPoints { get; set; }
    }
}
