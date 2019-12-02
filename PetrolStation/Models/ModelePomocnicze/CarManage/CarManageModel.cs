using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetrolStation.Models.ModelePomocnicze.CarManage
{
    public class CarManageModel
    {
        public Client client { get; set; } = new Client();
        public List<Car> cars { get; set; } = new List<Car>();

        //field to add new car to a client
        public Car carToAdd { get; set; } = new Car();
    }
}
