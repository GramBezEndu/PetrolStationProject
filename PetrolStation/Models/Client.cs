using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdClient { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string NIP { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string HouseNumber { get; set; }
        public int? ApartmentNumber { get; set; }
        [Required]
        public string Postcode { get; set; }
        [Required]
        public string Locality { get; set; }
    }
}
