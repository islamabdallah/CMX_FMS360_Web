using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Migrations.ApplicationDB;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Drawing;
using System.Net;
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
            //var trips = await _context.SapTrips.Where(t => t.IsVisible == true).GroupBy(e => e.TripNumber).ToListAsync();
            List<SapTripViewModel>models = new List<SapTripViewModel>();
            var trips = await _context.SapTrips
        .Where(t => t.IsVisible == true)
        .GroupBy(e => e.TripNumber)
        .Select(g => g.FirstOrDefault())
        .ToListAsync();
            if(trips.Count > 0)
            {
                foreach(var trip in trips)
                {
                    var tripViewModel = new SapTripViewModel
                    {
                        sapTrip = trip,
                        truckSilo = await _context.TruckSilos.Where(t => t.IsVisible == true && t.SapKey==trip.TruckNumber.ToString()).FirstOrDefaultAsync(),
                    };
                    models.Add(tripViewModel);
                }
            }
            return View("SapTrip",models);
        }

        public async Task<IActionResult> EditSapTrip(long id)
        {
            List<SapTripVModel> models = new List<SapTripVModel>();
            var trip = await _context.SapTrips.Where(t => t.Id == id).FirstOrDefaultAsync();
            if(trip == null)
            {
                return RedirectToAction("ERROR404");
            }
            SapTripVModel modell = new SapTripVModel();
            modell.sapTrip = trip;
            var materiall = await _context.Materials.Where(t => t.ProductId == Convert.ToInt64(modell.sapTrip.materialNumber)).FirstOrDefaultAsync();
            modell.MaterialName = materiall != null ? materiall.ProductNameAR : "";

            var trips = await _context.SapTrips.Where(t => t.TripNumber == trip.TripNumber).ToListAsync();
            var drivers = _driverService.GetAllDrivers();
            if (trips.Count > 0)
            {
                foreach(var tr in trips)
                {
                    SapTripVModel model = new SapTripVModel();
                    model.sapTrip = tr;
                    var material = await _context.Materials.Where(t => t.ProductId == Convert.ToInt64(model.sapTrip.materialNumber)).FirstOrDefaultAsync();
                    model.MaterialName = material != null ? material.ProductNameAR : "";
                   
                   // model.Drivers = drivers;
                    models.Add(model);
                }
            }
           
            ViewBag.DriverSelectList = drivers
    .Select(d => new SelectListItem
    {
        Value = d.DriverNumber.ToString(),
        Text = $"{d.DriverNumber} - {d.FullName}"
    })
    .ToList();
            ViewBag.truckSilo = await _context.TruckSilos.Where(t => t.IsVisible == true && t.SapKey == trip.TruckNumber).FirstOrDefaultAsync();
            ViewBag.trips = models;
            return View(modell);
        }
        public async Task<IActionResult> TripConvertDetails(long id)
        {
            SubTripDetailsViewModel subTrip = new SubTripDetailsViewModel();
            var trip = await _context.Trips.Where(t => t.Id == id).FirstOrDefaultAsync();
           
        subTrip.toLocations = new List<LocationApiModel>();
                subTrip.toLocations = await _context.PlannedTripLocations.Where(a =>a.Converted!=true && a.ParentTrip == trip.ParentTrip && a.TripNumber == trip.TripNumber && a.Type == "Dest") // Include AuthorId = 4 if needed
                .Select(a => new LocationApiModel
                {
                    tripLocationId = (int) a.Id,
                    customerName = a.customerName != null ? a.customerName : "",
                    customerPhoneNumber = a.customerPhoneNumber != null ? a.customerPhoneNumber : "",
                    recipientName = a.recipientName != null ? a.recipientName : "",
                    recipientPhoneNumber = a.recipientPhoneNumber != null ? a.recipientPhoneNumber : "",
                    locationStatus = a.locationStatus != null ? a.locationStatus : false,
                    address = a.Location,
                    materialType = a.Material,
                    lat = a.Lat,
                    lng = a.Long,
                    qty = a.Qty,
                    remainqty = a.Qty,
                    locationType = 2,
                    canConvert=true,
                    jobsiteId=a.JobSiteId

                })
                .ToListAsync();
            if (subTrip.toLocations.Count > 0)
            {
                //subTrip.toAddress = subTrip.toLocations[0].address;
                // subTrip.toAddress = "";
                foreach (var loc in subTrip.toLocations)
                {
                    var remain = await _context.ActualTripLocations.Where(a => a.IsVisible == true && a.PlannedTripLocationId == loc.tripLocationId).OrderBy(t => t.Id).LastOrDefaultAsync();
                    if (remain != null)
                    {

                        loc.remainqty = remain.Remain;
                    }

                    var actuals = await _context.ActualTripLocations.Where(t => t.IsVisible == true && t.PlannedTripLocationId == loc.tripLocationId).ToListAsync();
                    if (actuals.Count > 0)
                    {
                        foreach(var actual in actuals)
                        {
                            var log = await _context.TripLogs.Where(t => t.IsVisible == true && t.Id == actual.TripLogId).FirstOrDefaultAsync();
                            if(log != null)
                            {
                                if(log.Event== "StartUnLoading" || log.Event == "EndUnLoading" || log.Event == "Take5Step2")
                                {
                                    loc.canConvert = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (subTrip.toLocations.Count > 0)
            {
                //subTrip.toAddress = subTrip.toLocations[0].address;
                // subTrip.toAddress = "";
                subTrip.remainLocations = new List<long>();
                subTrip.AllRemainQty = 0;
                foreach (var loc in subTrip.toLocations)
                {
                   // var remain = await _context.ActualTripLocations.Where(a => a.IsVisible == true && a.PlannedTripLocationId == loc.tripLocationId).OrderBy(t => t.Id).LastOrDefaultAsync();
                    if (loc.locationStatus == true && loc.canConvert==true)
                    {
                        subTrip.remainLocations.Add((long)loc.tripLocationId);
                        subTrip.AllRemainQty += (double)loc.remainqty;
                    }
                }
            }
           // var homeData = await _tripService.GetTripDetailsForMobile(id, 1);
           
            var driversSelected=await _context.TripDrivers.Where(t=>t.ParentTrip==trip.ParentTrip && t.TripNumber==trip.TripNumber).Select(t=>t.DriverId).ToListAsync();
            subTrip.trip = trip;
            subTrip.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
            var drivers = _driverService.GetAllDrivers();
            ViewBag.DriverSelectList = drivers
     .Select(d => new SelectListItem
     {
         Value = d.DriverNumber.ToString(),
         Text = $"{d.DriverNumber} - {d.FullName}"
     })
     .ToList();

            ViewBag.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
            subTrip.driversNumber = driversSelected;

            return View(subTrip);
        }

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSapTrip(SapTripVModel model)
        {  
            var truck=await _context.TruckSilos.Where(t => t.IsVisible == true && t.SapKey == model.sapTrip.TruckNumber).FirstOrDefaultAsync();
            var trips= await _context.SapTrips.Where(t=>t.IsVisible==true && t.TripNumber==model.sapTrip.TripNumber).ToListAsync();
            var total = await _context.SapTrips.Where(t => t.IsVisible == true && t.TripNumber == model.sapTrip.TripNumber).SumAsync(t => t.Qty);
            TripModel trip = new TripModel()
            {
                TripNumber = model.sapTrip.TripNumber,
                TruckId = truck != null ? truck.Id : 0,
                TruckNumber = truck != null ? truck.TruckNumber : "",
                SiloNumber = truck != null ? truck.TruckNumber : "",
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
                ArrivedDate = (DateTime)model.sapTrip.departureDate,
                departureDate = (DateTime)model.sapTrip.departureDate
            };
            var jopSite = await _context.JobSites.Where(t => t.IsVisible == true && t.Number == model.sapTrip.jobsiteNumber).FirstOrDefaultAsync();
           
            trip.Distination = new List<JobSiteModel>();
            
            JobSiteModel jobSiteModel = new JobSiteModel()
            {

                Name = jopSite != null ? jopSite.Name : "",
                Number = model.sapTrip.jobsiteNumber,
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTripDate(long Parent, long TripNumber, DateTime DepartureDate)
        {
            var trip = await _context.Trips.Where(t => t.IsVisible == true && t.TripNumber == TripNumber && t.ParentTrip==Parent).FirstOrDefaultAsync();
            var truck =trip != null? await _context.Trucks.Where(t => t.IsVisible == true && t.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync(): null;
            if (trip != null)
            {
                trip.departureDate = DepartureDate;
                trip.UpdatedDate = DateTime.Now;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();
            }
            long truckId = truck != null ? truck.Id : 0;
            return RedirectToAction("Create", "Trip", new { truckId });
        }
        public ActionResult Createe()
        {
            try
            {
                TripModel model = new TripModel();
                model.Trucks = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                //model.Trucks.Insert(0, new TruckModel { Id = "select Truck" });
                model.JobSites = _jobsiteService.GetAllActiveJobsites().ToList();
                 var drivers = _driverService.GetAllDrivers();
                 model.Drivers = drivers;
                ViewBag.DriverSelectList = drivers
   .Select(d => new SelectListItem
   {
       Value = d.DriverNumber.ToString(),
       Text = $"{d.DriverNumber} - {d.FullName}"
   })
   .ToList();
                model.Date = DateTime.Now;
                return View("Create", model);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        // POST: TripController/Create
        //[HttpPost]
       // [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(long truckId)
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
                ViewBag.DriverSelectList = drivers
    .Select(d => new SelectListItem
    {
        Value = d.DriverNumber.ToString(),
        Text = $"{d.DriverNumber} - {d.FullName}"
    })
    .ToList();
                model.Date = DateTime.Now;
                var pendingTrips= await _tripService.GetAllPendingTripofTruckforMobile(truckId.ToString(), 1);
                //        var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber == truck.TruckNumber).ToListAsync();
                //        if (trips != null && trips.Count > 0)
                //        {
                //            model.TripGroup = trips.OrderBy(e => e.departureDate).GroupBy(e => e.ParentTrip)
                //                   .Select(async g => new TripGroupViewModel
                //                   {
                //                       ParentTrip = g.Key,
                //                       DepartureDate = _repository.Find(e => e.IsVisible == true && e.ParentTrip == g.Key).FirstOrDefaultAsync().Result.departureDate,
                //                       Trips = g.ToList(),
                //                       OnRoadDrivers = await _context.TripDrivers
                //.Where(t => t.IsVisible && t.ParentTrip == g.Key)
                //.Select(t => t.DriverId)
                //                                             .Distinct()
                //                                             .ToListAsync()
                //                   })
                //                   .ToList();
                var trips = await _repository.Find(e => e.IsVisible == true && e.StatusId != 3 && e.TruckNumber == truck.TruckNumber).ToListAsync();

                if (trips.Any())
                {
                    var tripGroups = trips.OrderBy(e => e.departureDate)
                                          .GroupBy(e => e.ParentTrip);

                    var tripGroupList = new List<TripGroupViewModel>();

                    foreach (var group in tripGroups)
                    {
                        var parentTrip = group.Key;

                        var departureTrip = await _repository.Find(e => e.IsVisible == true && e.ParentTrip == parentTrip)
                                                             .OrderBy(e => e.departureDate)
                                                             .FirstOrDefaultAsync();

                        var onRoadDrivers = await _context.TripDrivers
                                                          .Where(t => t.IsVisible && t.ParentTrip == parentTrip)
                                                          .Select(t => t.DriverId)
                                                          .Distinct()
                                                          .ToListAsync();

                        tripGroupList.Add(new TripGroupViewModel
                        {
                            ParentTrip = parentTrip,
                            DepartureDate = departureTrip?.departureDate ?? DateTime.MinValue,
                            Trips = group.ToList(),
                            OnRoadDrivers = onRoadDrivers
                        });
                    }

                    model.TripGroup = tripGroupList;
                


                //                var grouped = trips
                //.Where(e => e.IsVisible)
                //.OrderBy(e => e.departureDate)
                //.GroupBy(e => e.ParentTrip);

                //                model.TripGroup = (await Task.WhenAll(grouped.Select(async g =>
                //                {
                //                    var firstTrip = g.First();

                //                    var onRoadDrivers = await _context.TripDrivers
                //                        .Where(t => t.IsVisible && t.ParentTrip == g.Key)
                //                        .Select(t => t.DriverId)
                //                        .Distinct()
                //                        .ToListAsync();

                //                    return new TripGroupViewModel
                //                    {
                //                        ParentTrip = g.Key,
                //                        DepartureDate = firstTrip.departureDate,
                //                        Trips = g.ToList(),
                //                        OnRoadDrivers = onRoadDrivers
                //                    };
                //                }))).ToList(); // Convert array to List<T>

            }

                return View(model);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddResponse(MedicalResponseApiModel responseModel, List<JobSiteModel>? Destination, List<JobSiteModel>? Source)
        {
            try
            {
                var truckId = responseModel.TruckId;
                var lastparent = await _context.Trips.Where(t => t.ParentTrip == responseModel.ParentTrip).FirstOrDefaultAsync();
                if (lastparent != null) 
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
                    if(Destination != null)
                    {
                        if(Destination.Count>0)
                        {
                            foreach(var item in Destination)
                            {
                                var site=await _context.JobSites.Where(t=>t.IsVisible==true && t.Id==item.Id).FirstOrDefaultAsync();
                                if (site != null)
                                {
                                    PlannedTripLocation loc = new PlannedTripLocation()
                                    {
                                        JobSiteId = item.Id,
                                        ParentTrip = lastparent.ParentTrip,
                                        TripNumber = trip.TripNumber,
                                        TruckNumber = trip.TruckNumber,
                                        SiloNumber = trip.SiloNumber,
                                        Type = "Dest",
                                        Location = site.Name,
                                        Lat = site.Latitude,
                                        Long = site.Longitude,
                                        Material = item.Material,
                                        Qty = item.Qty,
                                        customerName = site.CustomerName,
                                        customerPhoneNumber = site.CustomerPhoneNumber,
                                        recipientName = site.RecipientName,
                                        recipientPhoneNumber = site.RecipientPhoneNumber,
                                        locationStatus = true,
                                        Converted = false,
                                        IsVisible = true,
                                        IsDelted = false,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now
                                    };
                                    _context.PlannedTripLocations.Add(loc);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    if (Source != null)
                    {
                        if (Source.Count > 0)
                        {
                            foreach (var item in Source)
                            {
                                var site = await _context.JobSites.Where(t => t.IsVisible == true && t.Id == item.Id).FirstOrDefaultAsync();
                                if (site != null)
                                {
                                    PlannedTripLocation loc = new PlannedTripLocation()
                                    {
                                        JobSiteId = item.Id,
                                        ParentTrip = lastparent.ParentTrip,
                                        TripNumber = trip.TripNumber,
                                        TruckNumber = trip.TruckNumber,
                                        SiloNumber = trip.SiloNumber,
                                        Type = "Source",
                                        Location = site.Name,
                                        Lat = site.Latitude,
                                        Long = site.Longitude,
                                        Material = item.Material,
                                        Qty = item.Qty,
                                        customerName = site.CustomerName,
                                        customerPhoneNumber = site.CustomerPhoneNumber,
                                        recipientName = site.RecipientName,
                                        recipientPhoneNumber = site.RecipientPhoneNumber,
                                        locationStatus = true,
                                        Converted = false,
                                        IsVisible = true,
                                        IsDelted = false,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now
                                    };
                                    _context.PlannedTripLocations.Add(loc);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
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
        public async Task<ActionResult> AddResponseConvert(ConvertResponseApiModel responseModel, List<JobSiteModel>? Destination, List<long>? Source)
        {
            try
            {
                long truckId = 8;
                List<long> locations = new List<long>();
                var trip = await _context.Trips.Where(t => t.IsVisible == true && t.TripNumber == responseModel.TripNumber).FirstOrDefaultAsync();
                if(trip != null)
                {
                    if (Destination != null)
                    {
                        if (Destination.Count > 0)
                        {
                            foreach (var item in Destination)
                            {
                                var site = await _context.JobSites.Where(t => t.IsVisible == true && t.Id == item.Id).FirstOrDefaultAsync();
                                if (site != null)
                                {
                                    PlannedTripLocation loc = new PlannedTripLocation()
                                    {
                                        JobSiteId = item.Id,
                                        ParentTrip = trip.ParentTrip,
                                        TripNumber = trip.TripNumber,
                                        TruckNumber = trip.TruckNumber,
                                        SiloNumber = trip.SiloNumber,
                                        Type = "Dest",
                                        Location = site.Name,
                                        Lat = site.Latitude,
                                        Long = site.Longitude,
                                        Material = item.Material,
                                        Qty = item.Qty,
                                        customerName = site.CustomerName,
                                        customerPhoneNumber = site.CustomerPhoneNumber,
                                        recipientName = site.RecipientName,
                                        recipientPhoneNumber = site.RecipientPhoneNumber,
                                        locationStatus = true,
                                        Converted = false,
                                        IsVisible = true,
                                        IsDelted = false,
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now
                                    };
                                    _context.PlannedTripLocations.Add(loc);
                                    await _context.SaveChangesAsync();
                                    if (loc.Id > 0)
                                    {
                                        locations.Add(loc.Id);
                                    }

                                }
                            }
                            TripConvert tripConvert = new TripConvert()
                            {
                                TripNumber = trip.TripNumber,
                                ParentTrip = trip.ParentTrip,
                                TripId = trip.Id,
                                TruckNumber = trip.TruckNumber,
                                SiloNumber = trip.SiloNumber,
                                createdby = "14869",
                                Date = DateTime.Now,
                                Status = "Convert",
                                CreatedDate=DateTime.Now,
                                UpdatedDate=DateTime.Now,
                                IsDelted=false,
                                IsVisible=true
                            };
                            _context.TripConverts.Add(tripConvert);
                            await _context.SaveChangesAsync();

                            if (Source != null)
                            {
                                if (Source.Count > 0)
                                {
                                    foreach (var item in Source)
                                    {
                                        var site = await _context.PlannedTripLocations.Where(t => t.IsVisible == true && t.Id == item).FirstOrDefaultAsync();
                                        if (site != null)
                                        {
                                            site.IsVisible = false;
                                            site.Converted = true;
                                            site.IsDelted = true;
                                            site.UpdatedDate = DateTime.Now;

                                            _context.PlannedTripLocations.Update(site);
                                            await _context.SaveChangesAsync();
                                            if(locations.Count > 0)
                                            {
                                                foreach (var location in locations)
                                                {
                                                    TripConvertLocation tripConvertLocation = new TripConvertLocation()
                                                    {
                                                        TripConvertId = tripConvert.Id,
                                                        OldLocId = site.Id,
                                                        NewLocId = location,
                                                        CreatedDate = DateTime.Now,
                                                        UpdatedDate = DateTime.Now,
                                                        IsDelted = false,
                                                        IsVisible = true
                                                    };
                                                    _context.TripConvertLocations.Add(tripConvertLocation);
                                                    await _context.SaveChangesAsync();
                                                }
                                            }
                                        }
                                    }
                                    trip.ConvertedSeen = false;
                                    trip.UpdatedDate = DateTime.Now;
                                    trip.IsConverted = true;
                                    _context.Trips.Update(trip);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    var truck = await _context.Trucks.Where(t => t.IsVisible == true && t.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync();
                    truckId = truck != null ? truck.Id : 8;
                }
                
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
        public async Task<ActionResult> AddDrivers(long Parent, List<long> drivers)
        {
            try
            {
                var trips = await _context.Trips.Where(t => t.IsVisible == true && t.ParentTrip == Parent).ToListAsync();

                long truckId =0;
                if (trips != null)
                {
                    if (trips.Count > 0)
                    {
                        var truck = await _context.Trucks.Where(t => t.IsVisible == true && t.TruckNumber == trips[0].TruckNumber).FirstOrDefaultAsync();
                        if (truck != null)
                        {
                            truckId = truck.Id;
                        }
                        if (drivers != null)
                        {
                            if (drivers.Count > 0)
                            {
                                foreach (var trip in trips)
                                {
                                    for (int i = 0; i < drivers.Count; i++)
                                    {
                                        TripDriver tripDriver = new TripDriver()
                                        {
                                            ParentTrip = Parent,
                                            TripNumber = trip.TripNumber,
                                            TruckNumber = trip.TruckNumber,
                                            SiloNumber = trip.SiloNumber,
                                            DriverId = drivers[i],
                                            Role = "OnRoad",
                                            CreatedDate = DateTime.Now,
                                            UpdatedDate = DateTime.Now,
                                            IsDelted = false,
                                            IsVisible = true
                                        };
                                        _context.TripDrivers.Add(tripDriver);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }

                        }
                    }
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
        public async Task<ActionResult> AddResponseParent(MedicalResponseApiModel responseModel, List<JobSiteModel>? Destination, List<JobSiteModel>? Source)
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
        public async Task<string> GetItemsss(string Id)
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


        [HttpPost]
        public async Task<string> GetItems(string Id,string type)
        {
            try
            {
                string[] insertedEmployeeNumbersString = new string[] { };
                if (Id != null && Id != "")
                {
                    string[] insertedEmployeeNumbersString2 = Id.Split(",");
                    insertedEmployeeNumbersString = insertedEmployeeNumbersString2.ToArray();
                }
                JobSiteVModel Items = new JobSiteVModel();
               Items.sites = new List<JobSite>();
                if (insertedEmployeeNumbersString != null)
                {
                    if (insertedEmployeeNumbersString.Length > 0)
                    {
                        foreach (var item in insertedEmployeeNumbersString)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var medItem = _context.JobSites.Where(t => t.Id == Convert.ToInt64(item)).FirstOrDefault();
                                Items.sites.Add(medItem);
                            }

                        }
                    }

                }
                string str = "";
                if (type == "2")
                {
                    str = "NonCement";
                    Items.materials = await _context.Materials.Where(t => t.IsVisible == true && t.Packing == "NonCement").ToListAsync();
                }
                else if (type == "1")
                {
                    Items.materials = await _context.Materials.Where(t => t.IsVisible == true && t.Packing != "NonCement" && t.Packing != "Mission").ToListAsync();
                }
                else
                {
                    str = "Mission";
                    Items.materials = await _context.Materials.Where(t => t.IsVisible == true && t.Packing == "Mission").ToListAsync();
                }
                //Items.materials = await _context.Materials.Where(t => t.IsVisible == true && t.Packing == str).ToListAsync();
                return JsonSerializer.Serialize(Items);

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IActionResult> AddScale(long id)
        {
            var trips = await _context.Trips.Where(t => t.SubTypeId == 1 && t.StatusId != 3 && t.StageEn== "Under Inspection").ToListAsync();
            if(trips != null)
            {
                if(trips.Count > 0)
                {
                    foreach (var trip in trips)
                    {
                         
                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddScale(SapTripVModel model)
        {
            var truck = await _context.TruckSilos.Where(t => t.IsVisible == true && t.SapKey == model.sapTrip.TruckNumber).FirstOrDefaultAsync();
            var trips = await _context.SapTrips.Where(t => t.IsVisible == true && t.TripNumber == model.sapTrip.TripNumber).ToListAsync();
            var total = await _context.SapTrips.Where(t => t.IsVisible == true && t.TripNumber == model.sapTrip.TripNumber).SumAsync(t => t.Qty);
            TripModel trip = new TripModel()
            {
                TripNumber = model.sapTrip.TripNumber,
                TruckId = truck != null ? truck.Id : 0,
                TruckNumber = truck != null ? truck.TruckNumber : "",
                SiloNumber = truck != null ? truck.TruckNumber : "",
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
                ArrivedDate = (DateTime)model.sapTrip.departureDate,
                departureDate = (DateTime)model.sapTrip.departureDate
            };
            var jopSite = await _context.JobSites.Where(t => t.IsVisible == true && t.Number == model.sapTrip.jobsiteNumber).FirstOrDefaultAsync();

            trip.Distination = new List<JobSiteModel>();

            JobSiteModel jobSiteModel = new JobSiteModel()
            {

                Name = jopSite != null ? jopSite.Name : "",
                Number = model.sapTrip.jobsiteNumber,
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
                model.sapTrip.UpdatedDate = DateTime.Now;
                _context.SapTrips.Update(model.sapTrip);
                await _context.SaveChangesAsync();
            }
            long idd = truck != null ? truck.Id : 0;
            return RedirectToAction("Create", "Trip", new { idd });
        }



    }
}
