using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class LoyalityCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdLoyalityCard { get; set; }
        public Client Client { get; set; }
        [ForeignKey("Client")]
        public int IdClient { get; set; }
        [Required]
        public int ActualPoints { get; set; }
    }
}
