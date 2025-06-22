using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

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
        private readonly ApplicationDBContext _context;

        public TripController(ITruckService truckService,
                              IJobSiteService jobsiteService,
                              IDriverService driverService,
                              ITripService tripService, IRepository<Trip, long> repository,
                              UserManager<ApplicationUser> userManager,
                              IEmployeeService employeeService, ITruckSiloService truckSiloService, ApplicationDBContext context)
        {
            _driverService = driverService;
            _truckService = truckService;
            _jobsiteService = jobsiteService;
            _tripService = tripService;
            _userManager = userManager;
            _employeeService = employeeService;
            _truckSiloService = truckSiloService;
            _repository = repository;
            _context = context;
        }
        public IActionResult Index()
        {
            return View("CreateTrip");
        }

        public async Task<IActionResult> SapTrip()
        {
            var trip = await _context.SapTrips.Where(t => t.IsVisible == true).ToListAsync();
            return View("SapTrip",trip);
        }

        public async Task<IActionResult> EditSapTrip(long id)
        {
            SapTripVModel model=new SapTripVModel();
            model.sapTrip = await _context.SapTrips.Where(t => t.Id == id).FirstOrDefaultAsync();
            var material=await _context.Materials.Where(t=>t.ProductId==Convert.ToInt64(model.sapTrip.materialNumber)).FirstOrDefaultAsync();
            model.MaterialName = material != null ? material.ProductNameAR : "";
            var drivers = _driverService.GetAllDrivers();
            model.Drivers = drivers;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSapTrip(SapTripVModel model)
        {
            //model.UpdatedDate = DateTime.Now;
            //model.IsActive = true;
            //model.IsVisible = true;
            //model.IsDelted = false;
            //var result3 = _repository.Update(model);
            var truck=await _context.Trucks.Where(t=>t.TruckNumber==model.sapTrip.TruckNumber).FirstOrDefaultAsync();
            TripModel trip = new TripModel()
            {
                TripNumber = model.sapTrip.TripNumber,
                TruckId = truck != null ? truck.Id : 0,
                TruckNumber = model.sapTrip.TruckNumber,
                SiloNumber = model.sapTrip.TruckNumber,
                TypeId = 1,
                SubTypeId = 1,
                Date = (DateTime)model.sapTrip.departureDate,
                StatusId = 1,
                StageEn = "Pending".Trim(),
                StageAR = "قيد الانتظار".Trim(),
                IsCanceled = false,
                IsConverted = false,
                FromPlant = true,
                Qty = model.sapTrip.Qty,
                IsDelted = false,
                IsVisible = true,
                loadDrivers = model.loadDrivers,
                onRoadDrivers = model.onRoadDrivers,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ArrivedDate = (DateTime)model.sapTrip.ArrivedDate,
                departureDate = (DateTime)model.sapTrip.departureDate
            };
            var jopSite = await _context.JobSites.Where(t => t.IsVisible == true && t.Number == Convert.ToInt64(model.sapTrip.jobsiteNumber)).FirstOrDefaultAsync();
            trip.Distination = new List<JobSiteModel>();
            JobSiteModel jobSiteModel = new JobSiteModel()
            {

                Name = jopSite != null ? jopSite.Name : "",
                Number = Convert.ToInt64(model.sapTrip.jobsiteNumber),
                Latitude = jopSite != null ? jopSite.Latitude : 0,

                Longitude = jopSite != null ? jopSite.Longitude : 0,
                City = jopSite != null ? jopSite.City : "",
                Desc = jopSite != null ? jopSite.Desc : "",

                HasNetworkCoverage = jopSite != null ? jopSite.HasNetworkCoverage : false,
                Material = model.MaterialName,
                Qty = model.sapTrip.Qty,
            };
            trip.Distination.Add(jobSiteModel);
            bool isTripAdded = _tripService.CreateTrip(trip).Result;
            if (isTripAdded)
            {
                model.sapTrip.IsVisible = false;
                model.sapTrip.UpdatedDate= DateTime.Now;
                _context.SapTrips.Update(model.sapTrip);
                await _context.SaveChangesAsync();
            }
            long idd = truck != null ? truck.Id : 0;
            return RedirectToAction("Create", "Trip", new {idd});
        }
        public ActionResult Create()
        {
            try
            {
                TripModel model = new TripModel();
                model.Trucks = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                //model.Trucks.Insert(0, new TruckModel { Id = "select Truck" });
                model.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
                 var drivers = _driverService.GetAllDrivers();
                 model.Drivers = drivers;
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
       // [ValidateAntiForgeryToken]
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
                var drivers = _driverService.GetAllDrivers().ToList();
                model.Drivers = drivers;
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
        public async Task<ActionResult> AddResponse(MedicalResponseApiModel responseModel)
        {
            try
            {
                var truckId = responseModel.TruckId;
                var lastparent = await _context.Trips.Where(t => t.ParentTrip == responseModel.ParentTrip).FirstOrDefaultAsync();
                if (lastparent != null) ;
                {
                    var lasttrip = await _context.Trips.Where(t => t.IsVisible == true && t.SubTypeId != 1).OrderBy(t => t.TripNumber).LastOrDefaultAsync();
                    var trip = new Trip()
                    {
                        ParentTrip = responseModel.ParentTrip,
                        TripNumber =lasttrip !=null?lasttrip.TripNumber+1: 1,
                        TruckNumber = lastparent.TruckNumber,
                        SiloNumber = lastparent.SiloNumber,
                        TypeId = lastparent.TypeId,
                        SubTypeId = responseModel.TypeId==3 && responseModel.Qty>0?4: responseModel.TypeId,
                        Date = responseModel.departureDate,
                        StatusId = 1,
                        StageEn = "Pending",
                        StageAR = "قيد الانتظار",
                        IsCanceled = false,
                        IsConverted = false,
                        MustStart = false,
                        FromPlant = false,
                        Qty = responseModel.Qty,
                        AssignQty = responseModel.Qty,
                        ArrivedDate = responseModel.ArrivedDate,

                        departureDate = responseModel.departureDate,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsDelted = false,
                        IsVisible = true,
                    };
                    _context.Trips.Add(trip);
                    await _context.SaveChangesAsync();
                }
                //return JsonSerializer.Serialize("Done");
                var redirectUrl = Url.Action("Create", "Trip", new { truckId });
                return Json(new { redirectUrl });
            }
            catch (Exception e)
            {
                //return null;
                return Json(new { success = false, message = "An error occurred." });
            }
        }




        [HttpPost]
        public async Task<ActionResult> AddResponseParent(MedicalResponseApiModel responseModel)
        {
            try
            {
                var truckId = responseModel.TruckId;
                var truck = await _context.Trucks.Where(t => t.IsVisible == true && t.Id == responseModel.TruckId).FirstOrDefaultAsync();
                var truckSilo = await _context.TruckSilos.Where(t => t.IsVisible == true && t.TruckNumber == truck.TruckNumber).FirstOrDefaultAsync();
                var lastparent = await _context.Trips.Where(t => t.IsVisible == true).OrderBy(t => t.ParentTrip).LastOrDefaultAsync();
                if (lastparent != null) ;
                {
                    var lasttrip = await _context.Trips.Where(t => t.IsVisible == true && t.SubTypeId != 1).OrderBy(t => t.TripNumber).LastOrDefaultAsync();
                    var trip = new Trip()
                    {
                        ParentTrip =lastparent !=null?lastparent.ParentTrip+1: responseModel.ParentTrip,
                        TripNumber = lasttrip != null ? lasttrip.TripNumber + 1 : 1,
                        TruckNumber = truck !=null?truck.TruckNumber : responseModel.TruckId.ToString(),
                        SiloNumber = truckSilo != null ? truckSilo.SiloNumber : truck.TruckNumber,
                        TypeId = responseModel.TypeId,
                        SubTypeId = responseModel.TypeId == 3 && responseModel.Qty > 0 ? 4 : responseModel.TypeId,
                        Date = responseModel.departureDate,
                        StatusId = 1,
                        StageEn = "Pending",
                        StageAR = "قيد الانتظار",
                        IsCanceled = false,
                        IsConverted = false,
                        MustStart = false,
                        FromPlant = false,
                        Qty = responseModel.Qty,
                        AssignQty = responseModel.Qty,
                        ArrivedDate = responseModel.ArrivedDate,

                        departureDate = responseModel.departureDate,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        IsDelted = false,
                        IsVisible = true,
                    };
                    _context.Trips.Add(trip);
                    await _context.SaveChangesAsync();
                }
                //return JsonSerializer.Serialize("Done");
                var redirectUrl = Url.Action("Create", "Trip", new { truckId });
                return Json(new { redirectUrl });
            }
            catch (Exception e)
            {
                //return null;
                return Json(new { success = false, message = "An error occurred." });
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

        [HttpPost]
        public async Task<string> GetItems(string Id)
        {
            try
            {
                string[] insertedEmployeeNumbersString = new string[] { };
                if (Id != null && Id != "")
                {
                    string[] insertedEmployeeNumbersString2 = Id.Split(",");
                    insertedEmployeeNumbersString = insertedEmployeeNumbersString2.ToArray();
                }
                List<JobSite> Items = new List<JobSite>();
                if (insertedEmployeeNumbersString != null)
                {
                    if (insertedEmployeeNumbersString.Length > 0)
                    {
                        foreach (var item in insertedEmployeeNumbersString)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var medItem = _context.JobSites.Where(t => t.Id == Convert.ToInt64(item)).FirstOrDefault();
                                Items.Add(medItem);
                            }

                        }
                    }

                }
                return JsonSerializer.Serialize(Items);

            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
