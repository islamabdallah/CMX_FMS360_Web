using FleetM360_DAL.Models;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FleetM360_Web.Controllers
{
    public class TripController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IJobSiteService _jobsiteService;
        private readonly IDriverService _driverService;
        private readonly ITripService _tripService;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IEmployeeService _employeeService;
        private readonly ITruckSiloService _truckSiloService;

        public TripController(ITruckService truckService,
                              IJobSiteService jobsiteService,
                              IDriverService driverService,
                              ITripService tripService,
                              UserManager<ApplicationUser> userManager,
                              IEmployeeService employeeService, ITruckSiloService truckSiloService)
        {
            _driverService = driverService;
            _truckService = truckService;
            _jobsiteService = jobsiteService;
            _tripService = tripService;
            _userManager = userManager;
            _employeeService = employeeService;
            _truckSiloService = truckSiloService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            try
            {
                TripModel model = new TripModel();
                model.Trucks = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                //model.Trucks.Insert(0, new TruckModel { Id = "select Truck" });
                model.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
                // var drivers = _driverService.GetAllActiveDrivers().ToList();
                // model.Drivers = drivers;
                model.Date = DateTime.Now;
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        // POST: TripController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TripModel model)
        {
            try
            {
               // TripModel model = new TripModel();
                model.Trucks = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                //model.Trucks.Insert(0, new TruckModel { Id = "select Truck" });
                model.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
                // var drivers = _driverService.GetAllActiveDrivers().ToList();
                // model.Drivers = drivers;
                model.Date = DateTime.Now;
                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateParentTrip(TripModel model)
        {
            try
            {

                bool isTripAdded = _tripService.CreateTrip(model).Result;

                if (isTripAdded == true)
                {
                    TempData["Message"] = "Success Process! trip has been added!";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Error"] = "Failed Process, Can not create trip";
                    return RedirectToAction("Create");
                }
            }
            catch
            {
                return RedirectToAction("ERROR404");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubTrip(TripModel model)
        {
            try
            {

                bool isTripAdded = _tripService.CreateTrip(model).Result;

                if (isTripAdded == true)
                {
                    TempData["Message"] = "Success Process! trip has been added!";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Error"] = "Failed Process, Can not create trip";
                    return RedirectToAction("Create");
                }
            }
            catch
            {
                return RedirectToAction("ERROR404");
            }
        }

        public async Task<string> IsTruckAvaliable(string truckId)
        {
            TripModel trip = new TripModel();//_tripService.GetPendingAndUnCompletedTripForTruck(truckId);
            var truck = _truckService.GetTruck(Convert.ToInt64(truckId));

            if (truck != null)
            {
                var truckSiloModel = _truckSiloService.GetLastActiveTruckSilo(truck.TruckNumber);
                return JsonSerializer.Serialize(truckSiloModel);
            }
            else
            {
                return null;
            }
        }
    }
}
