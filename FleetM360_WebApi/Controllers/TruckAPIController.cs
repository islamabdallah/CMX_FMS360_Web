using FleetM360_DAL.Models;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetM360_WebApi.Controllers
{
    // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class TruckAPIController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITripService _tripService;
        private readonly ILogger<TruckAPIController> _logger;
        private readonly ITruckService _truckService;
        private readonly IEmployeeService _employeeService;

        public TruckAPIController(IDriverService driverService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<TruckAPIController> logger,
            ITruckService truckService,
            IEmployeeService employeeService)
        {
            _driverService = driverService;
            _signInManager = signInManager;
            _userManager = userManager;
            _tripService = tripService;
            _logger = logger;
            _truckService = truckService;
            _employeeService = employeeService;
        }
        [HttpGet("pendingTrucks")]
        public async Task<ActionResult> pendingTrucks()
        {
            var trucks = _truckService.GetAllTrucksWithNoMobileAsync();
            if (trucks != null)
            {
                return Ok(new { Data = trucks, Message = "Successful Process" });
            }
            else
            {
                return Ok(new { Data = new List<TruckModel>(), Message = "Successful Process" });
            }
            // return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
        }

        [HttpPost("truckCollection")]
        public async Task<ActionResult> TruckCollection([Bind(include: "UserNumber")] LoginModel loginModel)
        {
            EmployeeModel employee = _employeeService.GetAdmin(loginModel.UserNumber);
            if (employee != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    var trucks = _truckService.GetAllTrucksWithNoMobileAsync().Result;
                    if (trucks != null)
                    {
                        List<TruckApiModel>truckList = new List<TruckApiModel>();
                        if (trucks.Count > 0)
                        {
                            foreach (var truck in trucks)
                            {
                                TruckApiModel truckApiModel = new TruckApiModel();
                                truckApiModel.truckNumber = truck.TruckNumber;
                                truckApiModel.truckId = truck.Id.ToString();
                                truckApiModel.truckStatus = truck.status;// "Pending";// truck.status;
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
                                truckList.Add(truckApiModel);
                            }
    }
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = truckList });
                    }
                    else
                    {
                        return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = new List<TruckApiModel>() });
                    }
                }
                return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        [HttpPost("assignTruck")]
        public async Task<ActionResult> assignTruck(AssignedTruckApiModel truckModel)
        {
            EmployeeModel employee = _employeeService.GetAdmin(truckModel.UserNumber);
            if (employee != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                   // var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, loginModel.Password, true, lockoutOnFailure: false);
                    var truckk = _truckService.GetTruck(Convert.ToInt64(truckModel.truckId));
                    if (truckk != null)
                    {
                        var truck = _truckService.GetPendingTruck(Convert.ToInt64(truckModel.truckId));
                        if (truck != null)
                        {
                            truck.DeviceId = truckModel.deviceId;
                            truck.UpdatedDate = DateTime.Now;
                            truck.PhoneNumber = truckModel.truckPhoneNumber;
                            truck.status = "Idle";
                            var updateResult = _truckService.UpdateTruck(truck).Result;
                            if (updateResult)
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
                                return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[truckModel.languageId], Data = truckApiModel });
                            }
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[truckModel.languageId], Data = 0 });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = truckMessage.AlreadyAssigned[truckModel.languageId], Data = 0 });
                        }
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = truckMessage.InvalidTruck[truckModel.languageId], Data = 0 });
                    }                                                               
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[truckModel.languageId], Data = 0 });
        }

        [HttpPost("truckStatus")]
        public async Task<ActionResult> truckstatus(TruckStatusApiModel truckModel)
        {
            if (truckModel == null)
                return BadRequest(new { flag = false, Message = "Error in truck Status", Data = 0 });
            var truck = _truckService.GetTruck(Convert.ToInt64(truckModel.truckId));
            if (truck != null)
            {
                truck.Lat = truckModel.lat;
                truck.Long = truckModel.lng;
                truck.UpdatedDate =DateTime.Now;
                var updateResult = _truckService.UpdateTruck(truck).Result;
                if (updateResult)
                    return Ok(new { flag = true, Message = "Truck Status Updated Successfully", Data = truckModel });

                return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[truckModel.languageId], Data = 0 });
            }
            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[truckModel.languageId], Data = 0 });
        }
    }
}
