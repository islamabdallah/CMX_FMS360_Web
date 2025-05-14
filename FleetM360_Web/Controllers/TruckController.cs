using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using FleetM360_Web.hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FleetM360_Web.Controllers
{
    public class TruckController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IHubContext<TruckHub> _hubContext;

        public TruckController(ITruckService truckService, IHubContext<TruckHub> hubContext)
        {
            _truckService = truckService;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            try
            {
                List<TruckModel> truckModels = _truckService.GetAllTrucks().ToList();
                return View(truckModels);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }
        // GET: TruckController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: TruckController/Create
        public ActionResult Create()
        {
            TruckModel truckModel = new TruckModel();
            return View(truckModel);
        }

        // POST: TruckController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TruckModel model)
        {
            try
            {

                bool result = _truckService.CreateTruck(model).Result;
                if (result == true)
                {
                    TempData["Message"] = "Truck Created Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Truck Created Successfully";
                    return View(model);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        // GET: TruckController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: TruckController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        // GET: TruckController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: TruckController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }
        public async Task<IActionResult> UpdateTruckLocation()
        {
            string truckId = "1233ي ا م ";// "1234ي ا م";//
            double lat = 27.2764339447021;//27.1664390563965;//
            double lng = 31.274621963501;//31.0157241821289;//
            await _hubContext.Clients.All.SendAsync("ReceiveTruckLocation", truckId, lat, lng);
            return Ok();
        }
        public async Task<IActionResult> UpdateTruckLocationn()
        {
            string truckId = "1234ي ا م";//
            double lat = 27.1664390563965;//
            double lng = 31.0157241821289;//
            await _hubContext.Clients.All.SendAsync("ReceiveTruckLocation", truckId, lat, lng);

            truckId = "1233ي ا م ";// "1234ي ا م";//
            lat = 27.2764339447021;//27.1664390563965;//
            lng = 31.274621963501;//31.0157241821289;//
            await _hubContext.Clients.All.SendAsync("ReceiveTruckLocation", truckId, lat, lng);
            return Ok();
        }
    }
    public class TruckLocationModel
    {
        public string TruckId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

}
