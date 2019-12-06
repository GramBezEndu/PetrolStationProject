using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PetrolStation.ExtensionMethods;
using PetrolStation.Models;
using PetrolStation.Models.ModelePomocnicze.CarManage;

namespace PetrolStation.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationContext _context;

        public ClientsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> ClientList()
        {
            ViewBag.IdClientsWithCard = await _context.LoyalityCard.ToListAsync();
            return View(await _context.Client.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult AddNewClient()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewClient([Bind("IdClient,Name,FirstName,Surname,NIP,Street,HouseNumber,ApartmentNumber,Postcode,Locality")] Client client, bool addCard)
        {
            if (ModelState.IsValid)
            {
                if((client.NIP==null && client.Name!=null)||(client.NIP != null && client.Name == null)
                    ||(client.FirstName==null && client.Surname!=null)||
                    (client.FirstName != null && client.Surname == null)||
                    (client.FirstName == null && client.Surname == null &&
                    client.NIP == null && client.Name == null))
                {
                    ViewBag.ErrorClient = "Invalid customer data";
                    return View(client);
                }
                if(client.NIP != null && client.Name != null && (client.FirstName != null || client.Surname != null))
                {
                    ViewBag.ErrorClient = "Invalid customer data";
                    return View(client);
                }
                if(client.FirstName != null && client.Surname != null && (client.NIP != null || client.Name != null))
                {
                    ViewBag.ErrorClient = "Invalid customer data";
                    return View(client);
                }

                _context.Add(client);
                await _context.SaveChangesAsync();
                //get added client from database
                if(addCard)
                {
                    var addedClient = _context.Client.Where(x => x.IdClient == client.IdClient).FirstOrDefault();
                    var newCard = new LoyalityCard()
                    {
                        IdClient = addedClient.IdClient,
                        ActualPoints = 0
                    };
                    _context.Add(newCard);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(ClientList));
            }
            return View(client);
        }

        public async Task<IActionResult> AddCard(int id)
        {
            var addedClient = _context.Client.Where(x => x.IdClient == id).FirstOrDefault();
            var newCard = new LoyalityCard()
            {
                IdClient = addedClient.IdClient,
                ActualPoints = 0
            };
            _context.Add(newCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ClientList));
        }
        //GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdClient,Name,FirstName,Surname,NIP,Street,HouseNumber,ApartmentNumber,Postcode,Locality")] Client client)
        {
            if (id != client.IdClient)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.IdClient))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ClientList));
            }
            return View(client);
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.IdClient == id);
        }

        [HttpGet]
        public IActionResult CarManage(int id)
        {
            CarManageModel carManageModel = new CarManageModel
            {
                client = _context.Client.Where(c => c.IdClient == id).ToList()[0],
                cars = _context.Car.Where(car => car.IdClient == id).ToList()
            };
            return View(carManageModel);
        }
        //add new car
        [HttpPost]
        public async Task<IActionResult> CarManage(CarManageModel carManageModel)
        {
            if (carManageModel.carToAdd.NumberPlate == null)
                return View(carManageModel);
            carManageModel.carToAdd.IdClient = carManageModel.client.IdClient;
            _context.Add(carManageModel.carToAdd);
            await _context.SaveChangesAsync();
            carManageModel.cars.Add(_context.Car.Where(c => c == carManageModel.carToAdd).ToList()[0]);
            TempData.Put<CarManageModel>("CarManageModel", carManageModel);
            ViewBag.Information = "Successfully added new car!";
            var id = carManageModel.client.IdClient;
            return RedirectToAction("CarManage",id);
        }

        public async Task<IActionResult> RemoveCar(int idCar)
        {
            var car = await _context.Car.FindAsync(idCar);
            int id = car.IdClient;
            car.IdClient = 1; //nie ma klienta
            _context.Update(car);
            await _context.SaveChangesAsync();
            ViewBag.Information = "Successfully removed car!";
            return RedirectToAction("CarManage", new RouteValueDictionary(new {controller="Clients", action="CarManage", id=id}));
        }
    }
}
