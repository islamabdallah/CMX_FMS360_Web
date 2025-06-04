using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRepository<Trip, long> _repository;
        private readonly IEmployeeService _employeeService;
        private readonly ITruckSiloService _truckSiloService;

        public TripController(ITruckService truckService,
                              IJobSiteService jobsiteService,
                              IDriverService driverService,
                              ITripService tripService, IRepository<Trip, long> repository,
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
            _repository = repository;
        }
        public IActionResult Index()
        {
            return View("CreateTrip");
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
        public async Task<ActionResult> CreateAsync(long truckId)
        {
            try
            {
                TripModel model = new TripModel();
                model.Trucks = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                var truck = _truckService.GetTruck(truckId);
                model.TruckId = truckId;
                model.TruckNumber = truck != null ? truck.TruckNumber : "";
                model.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
                // var drivers = _driverService.GetAllActiveDrivers().ToList();
                // model.Drivers = drivers;
                model.Date = DateTime.Now;
                var pendingTrips= await _tripService.GetAllPendingTripofTruckforMobile(truckId.ToString(), 1);
                var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber == truck.TruckNumber).ToListAsync();
                if (trips != null && trips.Count > 0)
                {
                    model.TripGroup = trips.OrderBy(e => e.departureDate).GroupBy(e => e.ParentTrip)
                           .Select(g => new TripGroupViewModel
                           {
                               ParentTrip = g.Key,
                               DepartureDate = _repository.Find(e => e.IsVisible == true && e.ParentTrip == g.Key).FirstOrDefaultAsync().Result.departureDate,
                               Trips = g.ToList()
                           })
                           .ToList();
                }
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

        public async Task<ActionResult> IsTruckAvaliable(long truckId)
        {
            TripModel trip = new TripModel();//_tripService.GetPendingAndUnCompletedTripForTruck(truckId);
            //var truck = _truckService.GetTruckByNumber(truckId);
            var truck = _truckService.GetTruck(truckId);

            if (truck != null)
            {
                var truckSiloModel = _truckSiloService.GetLastActiveTruckSilo(truck.TruckNumber);
                //return JsonSerializer.Serialize(truckSiloModel);
                return RedirectToAction("Create", "Trip", new { truckId });
            }
            else
            {
                return null;
            }
        }
    }
}
