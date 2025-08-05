using FirebaseAdmin.Messaging;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Socket;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using FleetM360_Web.hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace FleetM360_Web.Controllers
{
    public class TruckController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IHubContext<TruckHub> _hubContext;
        private readonly ApplicationDBContext _context;
        private readonly ITripService _tripService;
        private readonly WebSocketService _wsService;

        public TruckController(ITruckService truckService, IHubContext<TruckHub> hubContext, ApplicationDBContext context,
            WebSocketService wsService)
        {
            _truckService = truckService;
            _hubContext = hubContext;
            _context = context;
            _wsService = wsService;
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
        public async Task<string> getLocations()
        {
            try
            {
                var emp = await _context.Trucks.Where(t => t.IsVisible == true && t.status != "Not Assigned").ToListAsync();
                List<CurrentLocationModel> locations = new List<CurrentLocationModel>();
                foreach (var v in emp)
                {
                    string str = v.status;
                    if ((v.status == "Idle"))
                    {
                        var trips = await _context.Trips.Where(t => t.IsVisible == true && t.TruckNumber == v.TruckNumber && t.StatusId != 3).ToListAsync();
                        if (trips != null)
                        {
                            if (trips.Count > 0)
                            {
                                foreach (var tr in trips)
                                {
                                    if(tr.StageEn != "Pending")
                                    {
                                        var log=await _context.TripLogs.Where(t=>t.IsVisible == true && t.ParentTrip==tr.ParentTrip && t.TripNumber==tr.TripNumber)
                                            .OrderBy(t=>t.Id).LastOrDefaultAsync();
                                        if(log != null)
                                        {
                                            str = log.Event;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        str = "Not start Trip";
                                    }
                                }
                            }
                        }
                    }
                    CurrentLocationModel locationModel = new CurrentLocationModel();
                        locationModel.Id = v.Id;
                        locationModel.TruckNumber = v.TruckNumber;
                        locationModel.Status = str;
                        locationModel.Lat = v.Lat.ToString();
                        locationModel.Long = v.Long.ToString();
                        locationModel.Address = "";                     
                        locationModel.Date = v.UpdatedDate.ToString();
                        locations.Add(locationModel);
                    
                }

                // var item = _context.AttendanceDetails.Where(t => t.AttendanceId == Convert.ToInt32("1")).ToList();
                return JsonSerializer.Serialize(locations.OrderByDescending(t => t.Id).ToList());

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IActionResult> Release()
        {
            try
            {
                var truckModels = await _context.Trucks.Where(t => t.IsVisible == true && t.status.Trim() == "Maintainance".Trim()).ToListAsync(); //_truckService.GetAllTrucks().ToList();
                return View(truckModels);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        public async Task<IActionResult> TruckFailure()
        {
            try
            {
                var truckFailureModels = await _context.TruckFailures
                    .Where(t => t.IsVisible && t.Responsible == "Admin")
                    .Select(t => new TruckFailure
                    {
                        Id = t.Id,
                        TruckNumber = t.TruckNumber,
                        SiloNumber = t.SiloNumber,
                        DriverNumber = t.DriverNumber,
                        Category = t.Category,
                        Responsible = t.Responsible,
                        TripLogId = t.TripLogId,
                        ParentTrip = t.ParentTrip,
                        TruckFailures = t.TruckFailures
                            .Where(fd => fd.Stage != "End")
                            .ToList()
                    })
                    .ToListAsync();
                List< TruckFailureVModel > models = new List< TruckFailureVModel >();
                if (truckFailureModels != null)
                {
                    if(truckFailureModels.Count > 0)
                    {
                        foreach(var truck in truckFailureModels)
                        {
                            TruckFailureVModel model = new TruckFailureVModel();
                            model.trucks = truck;
                            model.hasActiveTrip = false;
                            var triplog=await _context.TripLogs.Where(t=>t.Id==truck.TripLogId).OrderBy(t => t.Id).LastOrDefaultAsync();
                            if(triplog != null)
                            {
                                model.activeTrip = await _context.Trips.Where(t => t.ParentTrip == triplog.ParentTrip && t.TripNumber == triplog.TripNumber).FirstOrDefaultAsync();
                                if(model.activeTrip != null)
                                {
                                    model.hasActiveTrip=true;
                                }
                            }
                            models.Add(model);
                        }
                    }
                }
                var Trucks = await _context.TruckSilos.ToListAsync(); //_truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();
                
                ViewBag.TruckSelectList = Trucks
   .Select(d => new SelectListItem
   {
       Value = d.SiloNumber,
       Text = d.TruckNumber,
   })
   .ToList();

                var truckSiloMap = Trucks
    .Where(t => !string.IsNullOrWhiteSpace(t.SiloNumber)) // ensure silo isn't null
    .ToDictionary(t => t.TruckNumber, t => t.SiloNumber);

                // JSON-encode it safely
                ViewBag.TruckSiloJson = JsonSerializer.Serialize(truckSiloMap);
                var Silos = _truckService.GetAllActiveTrucks().Where(t => t.Type == "Truck").ToList();

                ViewBag.SiloSelectList = Silos
   .Select(d => new SelectListItem
   {
       Value = d.TruckNumber,
       Text = d.TruckNumber,
   })
   .ToList();

                return View(models);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }

        }
        public async Task<IActionResult> ReleaseTruck(long id)
        {
           var model= await _context.Trucks.Where(t=>t.IsVisible==true && t.Id==id).FirstOrDefaultAsync();
            if (model == null)
            {
                return RedirectToAction("ERROR404");
            }
            model.status = "Idle";
            model.UpdatedDate = DateTime.Now;
            _context.Trucks.Update(model);
            await _context.SaveChangesAsync();
            var trips = await _context.Trips.Where(t => t.IsVisible == true && t.StatusId != 3 && t.TruckNumber == model.TruckNumber).ToListAsync();
            bool onTrip = false;
            if (trips != null)
            {
                if (trips.Count > 0)
                {
                    foreach (var trip in trips)
                    {
                        var triplog = await _context.TripLogs.Where(t => t.IsVisible == true && t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber).OrderBy(t => t.Id).LastOrDefaultAsync();
                        if (triplog != null)
                        {
                            if (triplog.Event == "Maintainance")
                            {
                                var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "EndMaintainance".Trim()).FirstOrDefault();
                                if (Event != null)
                                {
                                    TripLog tripLog = new TripLog()
                                    {
                                        ParentTrip = trip.ParentTrip,
                                        TripNumber = trip.TripNumber,
                                        Event = Event.LogName,
                                        LogId = Event.Id,
                                        // Lat = model.lat,
                                        // Long = model.lng,
                                        CreatedBy = "Admin",// model.UserNumber.ToString(),
                                        Date = DateTime.Now.ToString(),
                                        CreatedDate = DateTime.Now,
                                        UpdatedDate = DateTime.Now,
                                        IsDelted = false,
                                        IsVisible = true
                                    };
                                    _context.TripLogs.Add(tripLog);
                                    await _context.SaveChangesAsync();
                                    //trip.StageAR = StageAR;
                                    //trip.StageEn = StageEn;
                                    //trip.UpdatedDate = DateTime.Now;
                                    //_context.Trips.Update(trip);
                                    //await _context.SaveChangesAsync();
                                    SocketMessageApiModel message = new SocketMessageApiModel()
                                    {
                                        status = "maintenance_done",
                                        time = DateTime.Now,
                                    };

                                    await _wsService.SendMessageToUserAsync(Convert.ToInt32(triplog.CreatedBy), id.ToString(), JsonSerializer.Serialize(message));
                                    //return
                                    break;
                                }
                            }
                        }
                    }
                }

            }


            return RedirectToAction("Index");
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


        public async Task<IActionResult> ConvertTripAsync(long tripId, long truckId)
        {
            try
            {
                var truck = _truckService.GetTruck(truckId);
                if (truck != null)
                {
                    var trip= _context.Trips.Where(t=>t.Id == tripId).FirstOrDefault();
                    var trips = await _tripService.GetAllPendingTripofTruckforMobile(truckId.ToString(), 1);
                    return View(truck);
                }
                else
                {
                    TempData["Error"] = "Failed process, trip doesn't exist";
                    return RedirectToAction("SearchTrip");
                }

            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ConvertTrip(convertTruckModel model)
        //{
        //    try
        //    {
        //        bool result = false;
        //        string message = "";
        //      //  TripJobsiteModel tripJobsiteModel = _tripJobsiteService.GetTripJobsiteModelByTripNumberAndJobsiteId(model.TripId, model.ConvertedJobSiteId);
        //        //bool IsTripConverted = await _tripJobsiteService.ConvertTrip(model.TripId, tripJobsiteModel.JobSiteId);
        //        if (IsTripConverted == true)
        //        {
        //            if (tripJobsiteModel.TripStatus >= (int)CommanData.TripStatus.SurveyStepOneCompleted)
        //            {
        //                result = _surveyService.DeleteTake5StepOneForTripJobsite(model.TripId, tripJobsiteModel.JobSiteId);
        //            }
        //            else
        //            {
        //                result = true;
        //            }
        //            if (result == true)
        //            {
        //                result = _tripJobsiteService.CreateTripJobsite(model.TripId, model.JobSiteId).Result;
        //                if (result == true)
        //                {
        //                    TripModel tripModelAfterReset = _tripService.ResetTrip(model.TripId);
        //                    JobSiteModel jobSiteModel = _jobsiteService.GetJobsite(model.JobSiteId);
        //                    message = "تم تغيير موقع الرحلة رقم " + model.TripId + "الي " + jobSiteModel.Name;
        //                    Notification notification = _notificationService.CreateNotification(message, (int)CommanData.TripStatus.TripConverted, model.TripId, model.JobSiteId);
        //                    if (notification != null)
        //                    {
        //                        DriverModel driverModel = _driverService.GetDriver(tripModelAfterReset.DriverId);
        //                        UserNotificationModel addedUserNotificationModel = _userNotificationService.CreateUserNotification(notification.Id, driverModel.UserId).Result;
        //                        if (addedUserNotificationModel != null)
        //                        {
        //                            TempData["Message"] = "Trip is converted successfully";
        //                            return RedirectToAction("SearchTrip");
        //                        }
        //                        else
        //                        {
        //                            TempData["Message"] = "Trip is converted successfully, but can Send notification for driver";
        //                            return RedirectToAction("SearchTrip");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        TempData["Message"] = "Trip is converted successfully, but can Send notification";
        //                        return RedirectToAction("SearchTrip");
        //                    }
        //                }
        //                else
        //                {
        //                    TempData["Error"] = "failed add new jobsite for trip";
        //                }
        //            }
        //            else
        //            {
        //                TempData["Error"] = "Trip doesn't updated, failed to reset Take5 for trip";
        //            }
        //        }
        //        else
        //        {
        //            TempData["Error"] = "failed convert trip";
        //        }
        //        model.Jobsites = _jobsiteService.GetAllJobsites().ToList();
        //        return RedirectToAction("ConvertTrip");
        //    }
        //    catch (Exception e)
        //    {
        //        return RedirectToAction("ERROR404");
        //    }
        //}
    }
    public class TruckLocationModel
    {
        public string TruckId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class convertTruckModel
    {
        public long TruckId { get; set; }
        public List<TruckModel> trucks { get; set; }
        public List<DriverModel> drivers { get; set; }
        public long? convertedTruckId { get; set; }
    }

    public class CurrentLocationModel
    {
        public long Id { get; set; }      
        public string TruckNumber { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
       // public string Time { get; set; }
    }

}
