using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze
{
    public class FuelingModel
    {
        public Fueling fueling { get; set; } = new Fueling();
        public decimal VelueOfFueling { get; set; }
        public bool IsChecked { get; set; }

        public FuelingModel(Fueling fueling)
        {
            this.IsChecked = false;
            this.fueling = fueling;
        }
        public FuelingModel() { }
    }
}
