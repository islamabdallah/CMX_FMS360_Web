using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using FleetM360_Web.hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FleetM360_Web.Controllers
{
    public class TruckController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IHubContext<TruckHub> _hubContext;
        private readonly ApplicationDBContext _context;
        private readonly ITripService _tripService;

        public TruckController(ITruckService truckService, IHubContext<TruckHub> hubContext, ApplicationDBContext context)
        {
            _truckService = truckService;
            _hubContext = hubContext;
            _context = context;
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

}
