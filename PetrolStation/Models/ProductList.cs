using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class ProductList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTransaction { get; set; }
        public Product Product { get; set; }
        [ForeignKey("Product")]
        public int IdProduct { get; set; }
        [Required]
        public int Quantity { get; set; }

    }
}
