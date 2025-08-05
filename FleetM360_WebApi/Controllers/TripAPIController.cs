using FleetM360_DAL.Models;
using FleetM360_PLL.Services.Contracts.TermsConditions;
using FleetM360_PLL.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FleetM360_PLL.ViewModels;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Message;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_DAL.Models.MasterModels;
using Microsoft.EntityFrameworkCore;
using static FleetM360_PLL.CommanData;
using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_DAL.Migrations.ApplicationDB;
using FleetM360_PLL.ViewModels.Auth;
using FleetM360_DAL.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using FirebaseAdmin.Messaging;
using FleetM360_PLL.APIViewModels.Socket;
using System.Text.Json;
using FleetM360_PLL;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripAPIController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IEmployeeService _employeeService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITripService _tripService;
        private readonly ILogger<TripAPIController> _logger;
        private readonly ITruckService _truckService;
        private readonly IConfiguration _configuration;
        private readonly ITermsConditionsService _termsConditionsService;
        private readonly ApplicationDBContext _context;
        private readonly IPreCheckService _preCheckService;
        private readonly ITripLogService _tripLogService;
        private readonly WebSocketService _wsService;

        public TripAPIController(IDriverService driverService, IEmployeeService employeeService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<TripAPIController> logger,
            ITruckService truckService, IConfiguration configuration, ITermsConditionsService termsConditionsService, ApplicationDBContext context, IPreCheckService preCheckService, ITripLogService tripLogService,
            WebSocketService wsService)
        {
            _driverService = driverService;
            _employeeService = employeeService;
            _signInManager = signInManager;
            _userManager = userManager;
            _tripService = tripService;
            _logger = logger;
            _truckService = truckService;
            _configuration = configuration;
            _termsConditionsService = termsConditionsService;
            _context = context;
            _preCheckService = preCheckService;
            _tripLogService = tripLogService;
            _wsService = wsService;
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("startTrip")]//convert Done
        public async Task<ActionResult> startTrip([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip=_context.Trips.Where(a=>a.Id==Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        StartTripApiModel startTripApiModel = new StartTripApiModel();
                        var groupedTrips =await _tripService.GetHealthPrecheck(loginModel);
                        
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = groupedTrips });

                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("toolsPreCheck")] //convert Done
        public async Task<ActionResult> toolsPreCheck([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        QuestionDataModel groupedTrips = new QuestionDataModel();

                        //var groupedTrips = await _tripService.GetAllPendingTripofParentTrip();//.GetAllpendingTripGroupedByParentTrip();
                        groupedTrips.preCheckQuestions = await _tripService.GetToolsPrecheck(loginModel);
                        if(trip.IsConverted==true && trip.ConvertedSeen == false)
                        {
                            groupedTrips.isConverted = true;
                        }
                        else
                        {
                            groupedTrips.isConverted=false;
                        }
                            // return Ok(new { Data = groupedTrips, Message = "Successful Process" });
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = groupedTrips });
                        //return Ok(new { flag = true, isConverted=true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = groupedTrips });

                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getPreCheck")] //convert Done
        public async Task<ActionResult> getPreCheck([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        QuestionDataModel groupedTrips = new QuestionDataModel();

                        //var groupedTrips = await _tripService.GetAllPendingTripofParentTrip();//.GetAllpendingTripGroupedByParentTrip();
                        groupedTrips.preCheckQuestions = await _tripService.GetPrecheckListForCheck(loginModel);
                        if (trip.IsConverted == true && trip.ConvertedSeen == false)
                        {
                            groupedTrips.isConverted = true;
                        }
                        else
                        {
                            groupedTrips.isConverted = false;
                        }
                        // return Ok(new { Data = groupedTrips, Message = "Successful Process" });
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = groupedTrips });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("postPreCheckAnswers")] //convert Done
        public async Task<ActionResult> postPreCheckAnswers([Bind(include: "DriverNumber")] TripPreCheckApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                    if (trip != null) {
                        DataInfoApiModel startTripApiModel = new DataInfoApiModel();

                        //var groupedTrips = await _tripService.GetAllPendingTripofParentTrip();//.GetAllpendingTripGroupedByParentTrip();
                        var Result = await _preCheckService.AddTripPrecheck(loginModel);
                        startTripApiModel.route = "";
                        if (trip.IsConverted == true && trip.ConvertedSeen == false)
                        {
                            startTripApiModel.isConverted = true;
                        }
                        else
                        {
                            startTripApiModel.isConverted = false;
                        }

                        if (Result)
                        {                           
                            var onroaddriver =await _context.TripDrivers.Where(e => e.ParentTrip == trip.ParentTrip && e.Role == "OnRoad" && e.DriverId == loginModel.UserNumber).FirstOrDefaultAsync();
                            var loadedd =await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event == "EndGrossWeight").FirstOrDefaultAsync();
                            if (trip.SubTypeId == 1)// || trip.SubTypeId == 4)
                            {
                                if (onroaddriver != null && loginModel.category == 1)
                                {
                                    startTripApiModel.route = "PreCheckToolsScreen";
                                }
                                else if (onroaddriver != null && loginModel.category == 3 && loadedd != null)
                                {
                                    startTripApiModel.route = "WaitingPlantScreen";
                                }
                                else if (loginModel.category == 3 && onroaddriver != null && loadedd == null)
                                {
                                    startTripApiModel.route = "WeightDetailsPage";
                                }
                                else if (loginModel.category == 3 && onroaddriver == null)
                                {
                                    startTripApiModel.route = "TripsScreen";
                                }
                                else
                                {
                                    startTripApiModel.route = "PreCheckScreen";
                                }
                            }
                            else
                            {
                                if (onroaddriver != null && loginModel.category == 1)
                                {
                                    startTripApiModel.route = "PreCheckToolsScreen";
                                }
                                else if (onroaddriver != null && loginModel.category == 3)
                                {
                                    startTripApiModel.route = "WaitingPlantScreen";
                                }

                                else if (loginModel.category == 3 && onroaddriver == null)
                                {
                                    startTripApiModel.route = "TripsScreen";
                                }
                                else
                                {
                                    startTripApiModel.route = "PreCheckScreen";
                                }
                            }

                            if (loginModel.questionIds != null)
                            {
                                if (loginModel.questionIds.Count > 0)
                                {
                                    if (loginModel.category == 3)
                                    {
                                        startTripApiModel.route = "CarInspectionScreen";
                                    }
                                    else
                                    {
                                        startTripApiModel.route = "";
                                    }
                                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = startTripApiModel });
                                }
                                else
                                {
                                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = startTripApiModel });
                                }
                            }
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = startTripApiModel });
                        }

                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                    }
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("truckFaults")] //convert done
        public async Task<ActionResult> truckFaults([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {                       
                        var groupedTrips = await _tripService.GettruckFaults(loginModel);
                        if(groupedTrips != null)
                        {
                            if (trip.IsConverted == true && trip.ConvertedSeen == false)
                            {
                                groupedTrips.isConverted = true;
                            }
                            else
                            {
                                groupedTrips.isConverted = false;
                            }
                        }
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = groupedTrips });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getStopOptions")]////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Not yet
        public async Task<ActionResult> getStopOptions(UserApiModel loginModel)//(int languageId)
        {
            var trip =await _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefaultAsync();
            if (trip==null)
            {
                return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
            }
            StopOptionModel stopOptionModel = new StopOptionModel();
            var StopOptions = await _context.StopOptions.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
            .Select(a => new StopOptionApiModel
            {
                id = a.id.ToString(),
                label = loginModel.languageId == 1 ? a.Label_EN : a.Label_AR,
                iconBath = a.iconBath,
                color = a.color

            })
            .ToListAsync();
            stopOptionModel.StopOptions = StopOptions;
            if (trip.IsConverted == true && trip.ConvertedSeen == false)
            {
                stopOptionModel.isConverted = true;
            }
            else
            {
                stopOptionModel.isConverted = false;
            }
            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = stopOptionModel });


            //return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendStopData")]////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////Not yet
        public async Task<ActionResult> sendStopData(StopDataApiModel model)
        {
            
            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        if (model.stopOptionId == null || model.stopOptionId==0) {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        var Event=_context.LogLookups.Where(t=>t.IsVisible==true && t.LogName== "Stop").FirstOrDefault();
                        if (Event != null)
                        {
                            TripLog tripLog = new TripLog()
                            {
                                ParentTrip = trip.ParentTrip,
                                TripNumber = trip.TripNumber,
                                Event = Event.LogName,
                                LogId =model.stopOptionId,// Event.Id,
                                Lat = model.lat,
                                Long = model.lng,
                                CreatedBy = driver.DriverNumber.ToString(),
                                Date = DateTime.Now.ToString(),
                                StartDate = model.startDate,
                                EndDate = model.endDate,
                            };
                            _context.TripLogs.Add(tripLog);
                            await _context.SaveChangesAsync();
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
                        }
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });

                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getFuelData")]  //Convert done                           //have update from mobile side
        public async Task<ActionResult> getFuelData(UserApiModel loginModel)//(int languageId)
        {
            var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
            FuelDataModel model = new FuelDataModel();
             model.gasStations = await _context.GasStations.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
            .Select(a => new GasStationModel
            {
                id = a.Id.ToString(),
                name = a.name,
                lat = a.lat,
                lng = a.lng

            })
            .ToListAsync();
            model.cashPaymentMethodModel = await _context.PaymentMethods.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
           .Select(a => new CashPaymentMethodModel
           {
               id = a.Id.ToString(),
               name = a.name,
               icon = a.icon

           })
           .ToListAsync();
            model.isConverted = false;
            if(trip != null)
            {
                if(trip.IsConverted==true && trip.ConvertedSeen==false)
                {
                    model.isConverted = true;
                }
            }

            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = model });


            //return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendLoadingDriverComment")] // convert done
        public async Task<ActionResult> sendLoadingDriverComment([Bind(include: "DriverNumber")] loadingCommentApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        var tripLogg = await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.Event == "GrossWeight" && t.IsVisible == true).FirstOrDefaultAsync();
                        if (tripLogg != null)
                        {
                            var tripLog = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "EndGrossWeight").FirstOrDefault();
                            if (tripLog != null)
                            {
                                TripLog log = new TripLog()
                                {
                                    ParentTrip = trip.ParentTrip,
                                    TripNumber = trip.TripNumber,
                                    Event = "EndGrossWeight",
                                    LogId = tripLog.Id,
                                    Lat = loginModel.lat,
                                    Long = loginModel.lng,
                                    CreatedBy = loginModel.userNumber.ToString(),
                                    Comment = loginModel.loadingDriverComment,
                                    Date = DateTime.Now.ToString(),
                                    CreatedDate = DateTime.Now,
                                    UpdatedDate = DateTime.Now,
                                    IsDelted = false,
                                    IsVisible = true
                                };
                                _context.TripLogs.Add(log);
                                await _context.SaveChangesAsync();
                            }
                            //check if driver authorized to complete the trip go to toolsCheckScreen
                            string screen = "";

                            var roadModel = _context.TripDrivers.Where(e => e.ParentTrip == trip.ParentTrip && e.Role == "OnRoad" && e.DriverId == loginModel.userNumber).FirstOrDefaultAsync().Result;

                            if (roadModel == null)
                            {
                                screen = "TripsWidget";
                            }
                            else
                            {
                                screen = "PreCheckScreen";
                            }
                            convertCheckResult result = new convertCheckResult();
                            result.screen = screen;
                            if (trip.IsConverted == true && trip.ConvertedSeen == false)
                            {
                                result.isConverted = true;
                            }
                            else
                            {
                                result.isConverted = false;
                            }
                            // return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = screen });
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = result });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                        }
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.growth_Weight[loginModel.languageId], Data = 0 });
                    }
                }
            }
            else
            {
                return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
            }
            
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getTruckMaintenanceResult")]//convert done
        public async Task<ActionResult> getTruckMaintenanceResult([Bind(include: "DriverNumber")] loadingCommentApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver != null)
            {
                //var tripTruck = _context.Trucks.Where(t => t.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync().Result;
                //string num = tripTruck != null ? tripTruck.TruckNumber : "";
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        var tripLog = _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.IsVisible==true && (t.Event == "StartMaintainance" || t.Event == "Maintainance" || t.Event=="TruckConverted" || t.Event == "EndMaintainance")).OrderBy(t=>t.Id).LastOrDefaultAsync().Result;
                        // var groupedTrips = await _tripService.GettruckFaults(loginModel);
                        if (tripLog != null)
                        {
                            TruckApiModel truckApiModel = new TruckApiModel();
                            if(trip.IsConverted==true && trip.ConvertedSeen==false)
                            {
                                truckApiModel.isConverted = true;
                            }
                            else
                            {
                                truckApiModel.isConverted = false;
                            }
                            if (tripLog.Event == "StartMaintainance" || tripLog.Event == "Maintainance")
                            {
                                return Ok(new { flag = true, Message = UserMessage.startMaintainance[loginModel.languageId], Data = truckApiModel });
                            }
                            else if (tripLog.Event == "EndMaintainance")
                            {
                                return Ok(new { flag = true, Message = UserMessage.endMaintainance[loginModel.languageId], Data = truckApiModel });
                            }
                            else
                            {
                                var truck = _context.Trucks.Where(t => t.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync().Result;
                                if (truck != null)
                                {

                                    // TruckApiModel truckApiModel = new TruckApiModel();
                                    truckApiModel.truckNumber = truck.TruckNumber;
                                    truckApiModel.truckId = truck.Id.ToString();
                                    truckApiModel.truckStatus = truck.status; // "Not Assigned";// truck.status;
                                    truckApiModel.truckLocationLat = truck.Lat;
                                    truckApiModel.truckLocationLong = truck.Long;
                                    //truckApiModel.truckLastCheck = "";//truck.chec
                                    truckApiModel.truckLastLocation = truck.Location;
                                    truckApiModel.truckModel = truck.Model;
                                    truckApiModel.truckYear = truck.Year;
                                    truckApiModel.truckManufacturer = truck.TruckManufacturer;
                                    truckApiModel.truckChassis = truck.Chassis;
                                    truckApiModel.truckEngine = truck.Engine;
                                    truckApiModel.truckLicenseNumber = truck.LicenceNumber;
                                    truckApiModel.truckPhoneNumber = truck.PhoneNumber;
                                    truckApiModel.deviceId = truck.DeviceId;

                                    return Ok(new { flag = true, Message = UserMessage.truckReplaced[loginModel.languageId], Data = truckApiModel });
                                }
                                return Ok(new { flag = true, Message = UserMessage.truckReplaced[loginModel.languageId], Data = truckApiModel });//data=0
                            }

                                
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.failedMaintainance[loginModel.languageId], Data = 0 });
                        }

                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendFuelData")]//convert done
        public async Task<ActionResult> sendFuelData([FromForm] sendFuelDataApiModel model)
        {
            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        var Event = _tripLogService.CreateTrepFuelAsync(model).Result;
                        if (Event)
                        {
                            convertCheckResult result = new convertCheckResult();
                            if(trip.IsConverted==true && trip.ConvertedSeen==false)
                            {
                                result.isConverted = true;
                            }
                            else
                            {
                                result.isConverted = false;
                            }
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                        }
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getMaintenanceData")]//convert done
        public async Task<ActionResult> getMaintenanceData(UserApiModel loginModel)//(int languageId)
        {
            MaintenanceDataModel maintenanceData = new MaintenanceDataModel();
            maintenanceData.isConverted = false;
            var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
            if (trip != null)
            {
                if (trip.IsConverted == true && trip.ConvertedSeen == false)
                {
                    maintenanceData.isConverted = true;
                }
            }
            maintenanceData.waysToDealWithTruckBreakdowns = await _context.WayToDealWithTruckBreakdowns.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
           .Select(a => new WayToDealWithTruckBreakdownsModel
           {
               id = a.Id.ToString(),
               name = a.Name

           })
           .ToListAsync();
            
            maintenanceData.causesOfTruckFailure = await _context.CauseOfTruckFailures.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
          .Select(a => new CauseOfTruckFailureModel
          {
              id = a.Id.ToString(),
              name = a.Name

          })
          .ToListAsync();
            maintenanceData.responsibleOptions =new List<string>{ "قسم الصيانة","السائق"};

            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = maintenanceData });


        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("startMaintenance")] // convert done
        public async Task<ActionResult> startMaintenance(sendStopStartTime model)
        {
            
            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        if (model.type.Trim() == "Maintenance".Trim())
                        {
                            var waysToDealWithTruckBreakdowns =(model.wayOfDeal != null && model.wayOfDeal !="")? _context.WayToDealWithTruckBreakdowns.Where(t => t.Id == Convert.ToInt64(model.wayOfDeal)).FirstOrDefault() : null;
                            var causesOfTruckFailure = (model.causeOfFailure != null && model.causeOfFailure != "") ? _context.CauseOfTruckFailures.Where(t => t.Id == Convert.ToInt64(model.causeOfFailure)).FirstOrDefault() : null;
                            StopModel result = new StopModel()
                            {
                                causeOfFailure = causesOfTruckFailure != null ? causesOfTruckFailure.Name : "",
                                wayOfDeal = waysToDealWithTruckBreakdowns != null ? waysToDealWithTruckBreakdowns.Name : "",
                                driverComment = model.driverComment,
                                lat = model.lat,
                                lng = model.lng,
                                startTime = model.startTime,
                                type = model.type,
                                responsibleOption= model.responsibleOption,
                            };
                            
                            var Event = _tripLogService.CreateStartRoadMaintenanceAsync(model).Result;
                            if (Event)
                            {
                                if(trip.IsConverted==true && trip.ConvertedSeen==false)
                                {
                                    result.isConverted = true;
                                }
                                else
                                {
                                    result.isConverted = false;
                                }
                                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                            }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        else if (model.type.Trim() == "Ban".Trim())
                        {
                            //var waysToDealWithTruckBreakdowns = _context.WayToDealWithTruckBreakdowns.Where(t => t.Id == Convert.ToInt64(model.causeOfFailure)).FirstOrDefault();
                            var causesOfTruckFailure = _context.StopOptions.Where(t => t.id == Convert.ToInt64(model.causeOfFailure)).FirstOrDefault();
                            string reason = "";
                            if(causesOfTruckFailure != null)
                            {
                                reason = model.languageId == 1 ? causesOfTruckFailure.Label_EN : causesOfTruckFailure.Label_AR;
                            }
                            StopModel result = new StopModel()
                            {
                                causeOfFailure = reason,
                                wayOfDeal = "",
                                driverComment = model.driverComment,
                                lat = model.lat,
                                lng = model.lng,
                                startTime = model.startTime,
                                type = model.type,
                            };
                            var Event = _tripLogService.CreateStartStopBanAsync(model).Result;
                            if (Event)
                            {
                                if (trip.IsConverted == true && trip.ConvertedSeen == false)
                                {
                                    result.isConverted = true;
                                }
                                else
                                {
                                    result.isConverted = false;
                                }
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                            }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }                       
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });

                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("endMaintenance")]   // convert done
        public async Task<ActionResult> endMaintenance(sendStopStartTime model)
        {
            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {

                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        convertCheckResult result = new convertCheckResult();
                        if(trip.IsConverted ==true && trip.ConvertedSeen == false)
                        {
                            result.isConverted = true;
                        }
                        else
                        {
                            result.isConverted = false;
                        }
                        if (model.type.Trim() == "Maintenance".Trim())
                        {
                            var waysToDealWithTruckBreakdowns = (model.wayOfDeal != null && model.wayOfDeal != "") ? _context.WayToDealWithTruckBreakdowns.Where(t => t.Name == model.wayOfDeal).FirstOrDefault() : null;
                            var causesOfTruckFailure = (model.causeOfFailure != null && model.causeOfFailure != "") ? _context.CauseOfTruckFailures.Where(t => t.Name == model.causeOfFailure).FirstOrDefault() : null;
                            model.causeOfFailure = causesOfTruckFailure != null ? causesOfTruckFailure.Id.ToString() : "";
                            model.wayOfDeal = waysToDealWithTruckBreakdowns != null ? waysToDealWithTruckBreakdowns.Id.ToString() : "";

                            var tripLog = _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.IsVisible == true && (t.Event == "StartMaintainance" || t.Event == "StartRoadMaintenance" || t.Event == "TruckConverted" || t.Event == "EndMaintainance" || t.Event == "Maintainance")).OrderBy(t => t.Id).LastOrDefaultAsync().Result;
                            // var groupedTrips = await _tripService.GettruckFaults(loginModel);
                            if (tripLog != null)
                            {
                                
                                if (tripLog.Event == "StartMaintainance" || tripLog.Event == "Maintainance")
                                {
                                    result.maintenanceFeedback = UserMessage.roadStartMaintainance[model.languageId];
                                    return Ok(new { flag = true, Message = UserMessage.startMaintainance[model.languageId], Data = result });
                                }
                                else if (tripLog.Event == "EndMaintainance" ||tripLog.Event== "EndRoadMaintenance")
                                {
                                    result.maintenanceFeedback = UserMessage.roadEndMaintainance[model.languageId];
                                    return Ok(new { flag = true, Message = UserMessage.endMaintainance[model.languageId], Data = result });
                                }
                                else if (tripLog.Event == "TruckConverted")
                                {
                                    var truck = _context.Trucks.Where(t => t.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync().Result;
                                    if (truck != null)
                                    {

                                        TruckApiModel truckApiModel = new TruckApiModel();
                                        truckApiModel.truckNumber = truck.TruckNumber;
                                        truckApiModel.truckId = truck.Id.ToString();
                                        truckApiModel.truckStatus = truck.status; // "Not Assigned";// truck.status;
                                        truckApiModel.truckLocationLat = truck.Lat;
                                        truckApiModel.truckLocationLong = truck.Long;
                                        //truckApiModel.truckLastCheck = "";//truck.chec
                                        truckApiModel.truckLastLocation = truck.Location;
                                        truckApiModel.truckModel = truck.Model;
                                        truckApiModel.truckYear = truck.Year;
                                        truckApiModel.truckManufacturer = truck.TruckManufacturer;
                                        truckApiModel.truckChassis = truck.Chassis;
                                        truckApiModel.truckEngine = truck.Engine;
                                        truckApiModel.truckLicenseNumber = truck.LicenceNumber;
                                        truckApiModel.truckPhoneNumber = truck.PhoneNumber;
                                        truckApiModel.deviceId = truck.DeviceId;
                                        result.maintenanceFeedback = UserMessage.roadTruckReplaced[model.languageId];
                                        return Ok(new { flag = true, Message = UserMessage.truckReplaced[model.languageId], Data = result });
                                    }
                                    result.maintenanceFeedback = UserMessage.roadTruckReplaced[model.languageId];
                                    return Ok(new { flag = true, Message = UserMessage.truckReplaced[model.languageId], Data = result });
                                }
                                else
                                {
                                    var Event = _tripLogService.CreateEndRoadMaintenanceAsync(model).Result;
                                    if (Event)
                                    {
                                        result.maintenanceFeedback = UserMessage.roadEndMaintainance[model.languageId];
                                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                                    }
                                }
                            }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        else if (model.type.Trim() == "Ban".Trim())
                        {
                            // var waysToDealWithTruckBreakdowns = _context.WayToDealWithTruckBreakdowns.Where(t => t.Name == model.wayOfDeal).FirstOrDefault();
                            var causesOfTruckFailure = _context.StopOptions.Where(t => t.Label_EN == model.causeOfFailure || t.Label_AR == model.causeOfFailure).FirstOrDefault();
                            model.causeOfFailure = causesOfTruckFailure != null ? causesOfTruckFailure.id.ToString() : "";
                            model.wayOfDeal = "";//waysToDealWithTruckBreakdowns != null ? waysToDealWithTruckBreakdowns.Id.ToString() : "";
                            var Event = _tripLogService.CreateEndStopBanAsync(model).Result;
                            if (Event)
                            {
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                            }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                        }
                        

                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getTake5Data")]//convert done
        public async Task<ActionResult> getTake5Data([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                   // Take5APIDataModel model = new Take5APIDataModel();
                    var take5APIData = await _tripService.GetTake5DataForMobile(loginModel);
                    if (take5APIData != null)
                    {
                        var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                        take5APIData.isConverted = false;
                        if (trip != null)
                        {
                            if (trip.IsConverted == true && trip.ConvertedSeen == false)
                            {
                                take5APIData.isConverted = true;
                            }
                        }
                        return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = take5APIData });
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                    }
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendTake5Data")] //convert done
        public async Task<ActionResult> sendTake5Data(sendTake5DataApiModel model)
        {

            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {

                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {                                 
                        var Event = _tripLogService.CreateSiteProcessingAsync(model).Result;
                        if (Event)
                        {
                            bool isConverted = false;
                            if(trip.IsConverted==true && trip.ConvertedSeen == false)
                            {
                                isConverted = true;
                            }
                            bool subTripFlag = false;
                            bool parentFlag = false;
                            LocationApiModel _plant = new LocationApiModel()
                            {
                                address = "مصنع اسمنت اسيوط",
                                lat = 27.179130902288716,
                                lng = 31.022034339860536
                            };
                           
                            var actuallocations =await _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == model.tripLocationId).OrderBy(b => b.Id).LastOrDefaultAsync();
                            var planned=await _context.PlannedTripLocations.Where(t=>t.IsVisible==true && t.Id == model.tripLocationId).FirstOrDefaultAsync();
                            if(planned != null)
                            {
                                if(planned.locationStatus==false) //(actuallocations.Remain == 0)
                                {
                                    var subLocations =await _context.PlannedTripLocations.Where(t => t.IsVisible == true && t.TripNumber == actuallocations.TripNumber && t.ParentTrip==actuallocations.ParentTrip).ToListAsync();
                                    if(subLocations != null)
                                    {
                                        if (subLocations.Count > 0)
                                        {
                                            foreach (var location in subLocations)
                                            {
                                                //var actuallocation = _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == location.Id).OrderBy(b => b.Id).LastOrDefault();
                                                //if (actuallocation != null)
                                                //{
                                                //    if (actuallocation.Remain > 0 )
                                                //    {
                                                //        subTripFlag = false;
                                                //        parentFlag = false;
                                                //        break;
                                                //    }
                                                //    else
                                                //    {
                                                //        subTripFlag = true;
                                                //    }
                                                //}
                                                if (location.locationStatus==true)
                                                {
                                                    subTripFlag = false;
                                                    parentFlag = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    subTripFlag = true;
                                                }
                                            }
                                        }
                                    }
                                    if (subTripFlag == true)
                                    {
                                        trip.StageAR = "تم التسليم";
                                        trip.StageEn = "Completed";
                                        trip.UpdatedDate = DateTime.Now;
                                        _context.Trips.Update(trip);
                                        await _context.SaveChangesAsync();
                                        var allTrips=await _context.Trips.Where(t=>t.ParentTrip==actuallocations.ParentTrip).ToListAsync();
                                        if(allTrips.Count > 0)
                                        {
                                            foreach(var tr in  allTrips)
                                            {
                                                if(tr.StageEn !="Completed" && tr.StageEn != "Canceled")
                                                {
                                                    parentFlag = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    parentFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = new { parentFlag = parentFlag, subTripFlag, plant= _plant, isConverted= isConverted } });
                        }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });                        
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("truckArrivalData")] //convert done
        public async Task<ActionResult> truckArrivalData(TruckStatusApiModel model) // plant / site 
        {
            if (model == null)
                return BadRequest(new { flag = false, Message = "Error in truck Status", Data = 0 });

            //return Ok(new { flag = true, Message = "Truck Arrival Status updates Successfully", Data = truckModel });
            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.subTrip)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {

                        var Event = _tripLogService.CreateArriveSiteAsync(model).Result;
                        if (Event)
                        {
                            convertCheckResult result = new convertCheckResult();
                            result.screen = "";
                            if(trip.IsConverted==true && trip.ConvertedSeen == false)
                            {
                                result.isConverted = true;
                            }
                            else
                            {
                                result.isConverted = false;
                            }
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = result });
                        }
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("leavePlant")] //convert done
        public async Task<ActionResult> leavePlant([Bind(include: "userNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var trip = await _context.Trips.Where(t => t.Id == Convert.ToInt64(loginModel.tripId) && t.IsVisible == true).FirstOrDefaultAsync();
                    if (trip != null)
                    {
                        trip.StageAR = "جارى النقل";
                        trip.UpdatedDate = DateTime.Now;
                        trip.StageEn = "In Transit";
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();

                        var Eventt = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "LeavPlant").FirstOrDefault();
                        if (Eventt != null)
                        {
                            TripLog tripLog = new TripLog()
                            {
                                ParentTrip = trip.ParentTrip,
                                TripNumber = trip.TripNumber,
                                Event = Eventt.LogName,
                                LogId = Eventt.Id,
                                Lat = loginModel.lat,
                                Long = loginModel.lng,
                                CreatedBy = loginModel.UserNumber.ToString(),
                                Date = DateTime.Now.ToString(),
                                CreatedDate = DateTime.Now,
                                UpdatedDate = DateTime.Now,
                                IsDelted = false,
                                IsVisible = true
                            };
                            _context.TripLogs.Add(tripLog);
                            await _context.SaveChangesAsync();
                            convertCheckResult result = new convertCheckResult();
                            result.screen = "";
                            if(trip.IsConverted==true && trip.ConvertedSeen == false)
                            {
                                result.isConverted = true;
                            }
                            else
                            {
                                result.isConverted = false;
                            }

                            // return Ok(new { Data = homeData, Message = "Successful Process" });
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = result });
                        }
                    }
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendMaintenanceStartTime")]               //////////////////////////////////////////////////////////////////Not applied ///////////////////////for start corrective maintainance
        public async Task<ActionResult> sendMaintenanceStartTime([Bind(include: "userNumber")] sendMaintenanceEndTime loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver != null)
            {
                var truck = await _context.Trucks.Where(e => e.IsVisible == true && e.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync();
                if (truck != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        var trips = await _context.Trips.Where(a => a.TruckNumber == truck.TruckNumber).ToListAsync();
                        if (trips != null)
                        {
                            if (trips.Count > 0)
                            {
                                foreach (var trip in trips)
                                {
                                    var tripLog = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "Maintainance").FirstOrDefault();
                                    if (tripLog != null)
                                    {
                                        TripLog log = new TripLog()
                                        {
                                            ParentTrip = trip.ParentTrip,
                                            TripNumber = trip.TripNumber,
                                            Event = "Maintainance",
                                            LogId = tripLog.Id,
                                            Lat = loginModel.lat,
                                            Long = loginModel.lng,
                                            CreatedBy = loginModel.userNumber.ToString(),
                                            Date = DateTime.Now.ToString(),
                                            CreatedDate = DateTime.Now,
                                            UpdatedDate = DateTime.Now,
                                            IsDelted = false,
                                            IsVisible = true
                                        };
                                        _context.TripLogs.Add(log);
                                        await _context.SaveChangesAsync();
                                    }
                                    trip.StatusId = 3;
                                    trip.UpdatedDate = DateTime.Now;
                                    _context.Trips.Update(trip);
                                    await _context.SaveChangesAsync();
                                }
                            }

                            truck.status = "Maintainance";
                            truck.UpdatedDate = DateTime.Now;
                            _context.Trucks.Update(truck);
                            await _context.SaveChangesAsync();

                           // convertCheckResult result = new convertCheckResult();
                            

                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = 0 });
                        }
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
                    }
                   
                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendMaintenanceEndTime")] ///////////////////////Check if corrective maintainance end or not               
        public async Task<ActionResult> sendMaintenanceEndTime([Bind(include: "userNumber")] sendMaintenanceEndTime loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var truck = await _context.Trucks.Where(e => e.IsVisible == true && e.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync();
                    if (truck != null)
                    {
                        if (truck.status == "Maintainance")
                        {                            
                           return Ok(new { flag = true, Message = UserMessage.startMaintainance[loginModel.languageId], Data = "" });
                        }
                        else
                        {
                            return Ok(new { flag = true, Message = UserMessage.endMaintainance[loginModel.languageId], Data = "" });
                        }
                    }
                    // return Ok(new { Data = homeData, Message = "Successful Process" });
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 }); // FailedAccount
                  
                }
                return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 }); // FailedAccount
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendMaintenanceStartTimeislam")]
        public async Task<ActionResult> sendMaintenanceStartTimeislam([Bind(include: "userNumber")] sendMaintenanceEndTime loginModel)
        {
            //Driver 
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver ==null)
                return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount

            //Identity 
            ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
            if(aspNetUser == null)
                return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });

            //Truck
            var truck = await _context.Trucks.Where(e => e.IsVisible == true && e.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync();
            if (truck == null)
                return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });


            //Get Truck Active Parent trip "Single Record"
            var parentTrip =await _tripService.GetActiveParentTripOfTruck(truck.TruckNumber);
            var tripsToUpdate = _context.Trips
                               .Where(t => t.ParentTrip == parentTrip.ParentTrip)
                               .ToList();

            foreach (var trip in tripsToUpdate)
            {
                trip.StatusId = 3;
            }

            _context.SaveChanges();
            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = 0 });
           
            //return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("getWeightInfo")]
        public async Task<ActionResult> getWeightInfo(UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        WeightDataApiModel weightDataApiModel = new WeightDataApiModel();
                        weightDataApiModel.emptyWeight=new WeightApiModel();
                        weightDataApiModel.grossWeight=new WeightApiModel();
                        var tripLogg = await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.Event == "GrossWeight" && t.IsVisible == true).FirstOrDefaultAsync();
                        if (tripLogg != null)
                        {
                            weightDataApiModel.grossWeight.startTime = tripLogg.CreatedDate;
                            weightDataApiModel.grossWeight.endTime = tripLogg.CreatedDate;
                            weightDataApiModel.grossWeight.weight = trip.Qty;
                        }
                        var tripLog = await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.Event == "EmptyWeight" && t.IsVisible == true).FirstOrDefaultAsync();
                        if (tripLog != null)
                        {
                            weightDataApiModel.emptyWeight.startTime = tripLog.CreatedDate;
                            weightDataApiModel.emptyWeight.endTime = tripLog.CreatedDate;
                            weightDataApiModel.emptyWeight.weight = 26.480;
                        }
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = weightDataApiModel });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("covertedSeen")]
        public async Task<ActionResult> covertedSeen(UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(loginModel.tripId)).FirstOrDefault();
                if (trip != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        trip.ConvertedSeen=true;
                        trip.UpdatedDate = DateTime.Now;
                        _context.Trips.Update(trip);
                        await _context.SaveChangesAsync();
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("test")]
        public async Task<ActionResult> test([Bind(include: "userNumber")] sendMaintenanceEndTime loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.userNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    //var truck = await _truckRepository.Find(e => e.IsVisible == true && e.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync();
                    //if (truck != null)
                    //{
                    //    if (truck.status == "Maintainance")
                    //    {
                    //        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 }); // FailedAccount
                    //    }
                    //}
                    // return Ok(new { Data = homeData, Message = "Successful Process" });
                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = "" });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        
        [HttpPost("testSapTrip")]  //from sap
        public async Task<ActionResult> testSapTrip(SapTripVM model)
        {
            
            if (model != null)
            {
                SapTrip trip = new SapTrip()
                {
                    TripNumber =Convert.ToInt64(model.TripNumber),
                    Qty = Convert.ToInt64(model.Qty),
                    TruckNumber = model.TruckNumber,
                    jobsiteNumber = model.jobsiteNumber,
                    IsDelted=false,
                    IsVisible=true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    customerNumber = model.customerNumber,
                    materialNumber = model.materialNumber,
                    departureDate=model.departureDate,
                    ArrivedDate=model.ArrivedDate,
                };
                _context.SapTrips.Add(trip);
                await _context.SaveChangesAsync();
                return Ok(new { flag = true, Message = "Done Done ", Data = trip });
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[1], Data = 0 }); // FailedAccount
        }

        [HttpPost("sendTripWeight")]  //from sap
        public async Task<ActionResult> sendTripWeight(WeightSapModel model)
        {

            if (model != null)
            {
                var trip = await _context.Trips.Where(t => t.IsVisible && t.TripNumber == Convert.ToInt64(model.TripNumber)).FirstOrDefaultAsync();
                TripWeight tripp = new TripWeight()
                {
                    TripNumber = Convert.ToInt64(model.TripNumber),
                    ParentTrip = 1,
                    TruckNumber = model.TruckNumber,
                    CreatedBy = "Sap",
                    Weight = model.weight,
                    Type = model.WeightType,
                    IsDelted = false,
                    IsVisible = true,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                _context.tripWeights.Add(tripp);
                await _context.SaveChangesAsync();
                if (trip != null)
                {
                    var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == "GrossWeight".Trim()).FirstOrDefault();
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
                            CreatedBy = "sap",// model.UserNumber.ToString(),
                            Date = DateTime.Now.ToString(),
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsDelted = false,
                            IsVisible = true
                        };
                        _context.TripLogs.Add(tripLog);
                        await _context.SaveChangesAsync();

                        //return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
                    }
                    trip.StageAR = "قيد الفحص";
                    trip.StageEn = "Under Inspection";
                    trip.UpdatedDate = DateTime.Now;
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();
                    var truck = await _context.Trucks.Where(t => t.IsVisible == true && t.TruckNumber == trip.TruckNumber).FirstOrDefaultAsync();
                    if (truck != null)
                    {
                        var drivers=await _context.TripDrivers.Where(t=>t.TripNumber == trip.TripNumber).ToListAsync();
                        if(drivers != null)
                        {
                            if(drivers.Count > 0)
                            {
                                SocketMessageApiModel message=new SocketMessageApiModel();
                                message.status = "gross_weight_ended";
                                message.time = DateTime.Now;
                                message.Weight_value=model.weight.ToString();
                                foreach (var driver in drivers)
                                {
                                    await _wsService.SendMessageToUserAsync((int)driver.DriverId, truck.Id.ToString(), JsonSerializer.Serialize(message));
                                    return Ok("Notification sent.");
                                }
                            }
                        }
                    }
                   // await _wsService.SendMessageToUserAsync(userNumber, truckId, JsonSerializer.Serialize(message));
                    
                }

                return Ok(new { flag = true, Message = "Done Done ", Data = trip });
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            //send to admin
            //return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[1], Data = 0 }); // FailedAccount
            //sen admin notification

            return Ok(new { flag = true, Message = "Done Done ", Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("sendTake5DataByStage")]
        public async Task<ActionResult> sendTake5DataByStage(sendTake5DataByStageApiModel model)
        {

            DriverModel driver = _driverService.GetDriver(model.userNumber);
            if (driver != null)
            {
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(model.tripId)).FirstOrDefault();
                if (trip != null)
                {

                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {
                        var loc = await _context.PlannedTripLocations.Where(t => t.Id == model.tripLocationId).FirstOrDefaultAsync();
                        if (model.stage == 1)
                        {
                            
                            if(loc != null)
                            {
                                if (loc.Converted == true)
                                {
                                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = new { isConverted=true } });
                                }
                            }
                        }
                        var Event = _tripLogService.CreateSiteProcessingByStageAsync(model).Result;
                        if (Event)
                        {
                            //start reverse convert
                            if (loc != null)
                            {
                                if (loc.Converted == true)
                                {
                                    var convertion = await _context.TripConvertLocations.Where(t => t.IsVisible == true && t.OldLocId == loc.Id).OrderBy(t => t.Id).LastOrDefaultAsync();
                                    if(convertion != null)
                                    {
                                        var convertionList = await _context.TripConvertLocations.Where(t => t.IsVisible == true && t.TripConvertId == convertion.TripConvertId).ToListAsync();
                                        if(convertionList != null)
                                        {
                                            if (convertionList.Count > 0)
                                            {
                                                foreach(var item in convertionList)
                                                {
                                                    var old = await _context.PlannedTripLocations.Where(t => t.Id == item.OldLocId).FirstOrDefaultAsync();
                                                    var last = await _context.PlannedTripLocations.Where(t => t.Id == item.NewLocId).FirstOrDefaultAsync();
                                                    if(old != null)
                                                    {
                                                        old.IsDelted = false;
                                                        old.IsVisible = true;
                                                        old.Converted = false;
                                                        old.UpdatedDate = DateTime.Now;
                                                        _context.PlannedTripLocations.Update(old);
                                                        await _context.SaveChangesAsync();
                                                    }
                                                    if (last != null)
                                                    {
                                                        last.IsDelted = false;
                                                        last.IsVisible = true;
                                                        last.Converted = false;
                                                        last.UpdatedDate = DateTime.Now;
                                                        _context.PlannedTripLocations.Update(last);
                                                        await _context.SaveChangesAsync();
                                                    }
                                                }
                                            }
                                           
                                        }
                                        trip.IsConverted = false;
                                        trip.ConvertedSeen = true;
                                        trip.UpdatedDate = DateTime.Now;
                                        _context.Trips.Update(trip);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                            //End reverse convert

                            bool subTripFlag = false;
                            bool parentFlag = false;
                            LocationApiModel _plant = new LocationApiModel()
                            {
                                address = "مصنع اسمنت اسيوط",
                                lat = 27.179130902288716,
                                lng = 31.022034339860536
                            };
                            var arrived = await _context.TripLogs.Where(t => t.IsVisible == true && t.ParentTrip == trip.ParentTrip && t.Event == "AutomaticArrivePlant").FirstOrDefaultAsync();
                            _plant.arrivalFlag = arrived != null ? true : false;
                            var actuallocations = await _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == model.tripLocationId).OrderBy(b => b.Id).LastOrDefaultAsync();
                            var planned = await _context.PlannedTripLocations.Where(t => t.IsVisible == true && t.Id == model.tripLocationId).FirstOrDefaultAsync();
                            if (planned != null)
                            {
                                if (planned.locationStatus == false && model.stage==4) //(actuallocations.Remain == 0)
                                {
                                    var subLocations = await _context.PlannedTripLocations.Where(t => t.IsVisible == true && t.TripNumber == actuallocations.TripNumber && t.ParentTrip == actuallocations.ParentTrip).ToListAsync();
                                    if (subLocations != null)
                                    {
                                        if (subLocations.Count > 0)
                                        {
                                            foreach (var location in subLocations)
                                            {
                                                if (location.locationStatus == true)
                                                {
                                                    subTripFlag = false;
                                                    parentFlag = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    subTripFlag = true;
                                                }
                                            }
                                        }
                                    }
                                    if (subTripFlag == true)
                                    {
                                        trip.StageAR = "تم التسليم";
                                        trip.StageEn = "Completed";
                                        trip.UpdatedDate = DateTime.Now;
                                        _context.Trips.Update(trip);
                                        await _context.SaveChangesAsync();
                                        var allTrips = await _context.Trips.Where(t => t.ParentTrip == actuallocations.ParentTrip).ToListAsync();
                                        if (allTrips.Count > 0)
                                        {
                                            foreach (var tr in allTrips)
                                            {
                                                if (tr.StageEn != "Completed" && tr.StageEn != "Canceled")
                                                {
                                                    parentFlag = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    parentFlag = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = new { parentFlag = parentFlag, subTripFlag, plant = _plant } });
                        }
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[model.languageId], Data = 0 });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("covertedtest")]
        public async Task<ActionResult> covertedtest(long tripId)
        {
            var trip = _context.Trips.Where(a => a.Id ==tripId).FirstOrDefault();
            if (trip != null)
            {

                trip.ConvertedSeen = false;
                trip.IsConverted = true;
                trip.UpdatedDate = DateTime.Now;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();
                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[1], Data = 0 });

            }
            else
            {
                return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[1], Data = 0 });
            }
         
        }
    }

}
