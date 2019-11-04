using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTransaction { get; set; }
        public LoyalityCard LoyalityCard { get; set; }
        [ForeignKey("LoyalityCard")]
        public virtual int? IdLoyalityCard { get; set; }
        [Required]
        public DateTime Date { get; set; }

    }
}
