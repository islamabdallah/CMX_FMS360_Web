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

        public TripAPIController(IDriverService driverService, IEmployeeService employeeService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<TripAPIController> logger,
            ITruckService truckService, IConfiguration configuration, ITermsConditionsService termsConditionsService, ApplicationDBContext context, IPreCheckService preCheckService, ITripLogService tripLogService)
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
        }
        [HttpPost("startTrip")]
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

        [HttpPost("toolsPreCheck")]
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

        [HttpPost("getPreCheck")]
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

        [HttpPost("postPreCheckAnswers")]
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

                        if (Result)
                        {                           
                            var onroaddriver =await _context.TripDrivers.Where(e => e.ParentTrip == trip.ParentTrip && e.Role == "OnRoad" && e.DriverId == loginModel.UserNumber).FirstOrDefaultAsync();
                            var loadedd =await _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.IsVisible == true && t.Event == "EndGrossWeight").FirstOrDefaultAsync();

                            if (onroaddriver != null && loginModel.category==1 && loadedd !=null)
                            {
                                startTripApiModel.route = "PreCheckToolsScreen";
                            }
                            if (onroaddriver != null && loginModel.category == 3 && loadedd != null)
                            {
                                startTripApiModel.route = "WaitingPlantScreen";
                            }
                            else if(loginModel.category==3 && onroaddriver != null && loadedd == null)
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

        [HttpPost("truckFaults")]
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

        [HttpPost("getStopOptions")]
        public async Task<ActionResult> getStopOptions(int languageId)
        {
            var StopOptions = await _context.StopOptions.Where(a => a.IsVisible == true) // Include AuthorId = 4 if needed
            .Select(a => new StopOptionApiModel
            {
                id = a.id.ToString(),
                label = languageId == 1 ? a.Label_EN : a.Label_AR,
                iconBath = a.iconBath,
                color = a.color

            })
            .ToListAsync();

            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[languageId], Data = StopOptions });


            //return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }
        [HttpPost("sendStopData")]
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

        [HttpPost("getFuelData")]
        public async Task<ActionResult> getFuelData(int languageId)
        {
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

            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[languageId], Data = model });


            //return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [HttpPost("sendLoadingDriverComment")]
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
                        var tripLogg=await _context.TripLogs.Where(t=>t.ParentTrip==trip.ParentTrip && t.TripNumber==trip.TripNumber && t.Event== "GrossWeight" && t.IsVisible==true).FirstOrDefaultAsync();
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
                                    LogId = 3,
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
                                screen = "PreCheckToolsScreen";
                            }
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = screen });
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
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }
        
        [HttpPost("getTruckMaintenanceResult")]
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
                        var tripLog = _context.TripLogs.Where(t => t.ParentTrip == trip.ParentTrip && t.TripNumber == trip.TripNumber && t.Event == "StartMaintainance" || t.Event=="TruckConverted" || t.Event == "EndMaintainance").OrderBy(t=>t.Id).LastOrDefaultAsync().Result;
                        // var groupedTrips = await _tripService.GettruckFaults(loginModel);
                        if (tripLog != null)
                        {
                          if(tripLog.Event== "StartMaintainance")
                            {
                                return Ok(new { flag = true, Message = UserMessage.startMaintainance[loginModel.languageId], Data = new TruckApiModel() });
                            }
                          else if (tripLog.Event == "EndMaintainance")
                            {
                                return Ok(new { flag = true, Message = UserMessage.endMaintainance[loginModel.languageId], Data = new TruckApiModel() });
                            }
                            else
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
                                    
                                    return Ok(new { flag = true, Message = UserMessage.truckReplaced[loginModel.languageId], Data = truckApiModel });
                                }
                                return Ok(new { flag = true, Message = UserMessage.truckReplaced[loginModel.languageId], Data = 0 });
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
        
        [HttpPost("sendFuelData")]
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

        [HttpPost("getMaintenanceData")]
        public async Task<ActionResult> getMaintenanceData(int languageId)
        {
            MaintenanceDataModel maintenanceData = new MaintenanceDataModel();
           
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

            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[languageId], Data = maintenanceData });


        }

        [HttpPost("startMaintenance")]
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
                            var waysToDealWithTruckBreakdowns = _context.WayToDealWithTruckBreakdowns.Where(t => t.Id == Convert.ToInt64(model.causeOfFailure)).FirstOrDefault();
                            var causesOfTruckFailure = _context.CauseOfTruckFailures.Where(t => t.Id == Convert.ToInt64(model.causeOfFailure)).FirstOrDefault();
                            StopModel result = new StopModel()
                            {
                                causeOfFailure = causesOfTruckFailure != null ? causesOfTruckFailure.Name : "",
                                wayOfDeal = waysToDealWithTruckBreakdowns != null ? waysToDealWithTruckBreakdowns.Name : "",
                                driverComment = model.driverComment,
                                lat = model.lat,
                                lng = model.lng,
                                startTime = model.startTime,
                                type = model.type,
                            };
                            var Event = _tripLogService.CreateStartRoadMaintenanceAsync(model).Result;
                            if (Event)
                            {
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

        [HttpPost("endMaintenance")]
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
                        if (model.type.Trim() == "Maintenance".Trim())
                        {
                            var waysToDealWithTruckBreakdowns = _context.WayToDealWithTruckBreakdowns.Where(t => t.Name == model.wayOfDeal).FirstOrDefault();
                            var causesOfTruckFailure = _context.CauseOfTruckFailures.Where(t => t.Name == model.causeOfFailure).FirstOrDefault();
                            model.causeOfFailure = causesOfTruckFailure != null ? causesOfTruckFailure.Id.ToString() : "";
                            model.wayOfDeal = waysToDealWithTruckBreakdowns != null ? waysToDealWithTruckBreakdowns.Id.ToString() : "";
                            var Event = _tripLogService.CreateEndRoadMaintenanceAsync(model).Result;
                            if (Event)
                            {
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
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
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
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

        [HttpPost("getTake5Data")]
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
                        //return Ok(new { Data = groupedTrips, Message = "Successful Process" });
                        return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = take5APIData });
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                    }
                }
            }
            // return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [HttpPost("sendTake5Data")]
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
                            bool subTripFlag=false;
                            bool parentFlag = false;
                            LocationApiModel _plant = new LocationApiModel()
                            {
                                address = "مصنع اسمنت اسيوط",
                                lat = 27.179130902288716,
                                lng = 31.022034339860536
                            };
                           
                            var actuallocations =await _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == model.tripLocationId).OrderBy(b => b.Id).LastOrDefaultAsync();
                            if(actuallocations != null)
                            {
                                if (actuallocations.Remain == 0)
                                {
                                    var subLocations =await _context.PlannedTripLocations.Where(t => t.IsVisible == true && t.TripNumber == actuallocations.TripNumber && t.ParentTrip==actuallocations.ParentTrip && t.Type== "Dest").ToListAsync();
                                    if(subLocations != null)
                                    {
                                        if (subLocations.Count > 0)
                                        {
                                            foreach (var location in subLocations)
                                            {
                                                var actuallocation = _context.ActualTripLocations.Where(b => b.PlannedTripLocationId == model.tripLocationId).OrderBy(b => b.Id).LastOrDefault();
                                                if (actuallocation != null)
                                                {
                                                    if (actuallocation.Remain > 0)
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
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = new { parentFlag = parentFlag, subTripFlag, plant= _plant } });
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

        [HttpPost("truckArrivalData")]
        public async Task<ActionResult> truckArrivalData(TruckStatusApiModel model)
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
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[model.languageId], Data = 0 });
        }


        [HttpPost("leavePlant")]
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


                            // return Ok(new { Data = homeData, Message = "Successful Process" });
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = "" });
                        }
                    }
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [HttpPost("sendMaintenanceStartTime")]
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
                                            LogId = 3,
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

        [HttpPost("sendMaintenanceEndTime")]
        public async Task<ActionResult> sendMaintenanceEndTime([Bind(include: "userNumber")] sendMaintenanceEndTime loginModel)
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


    }
}
