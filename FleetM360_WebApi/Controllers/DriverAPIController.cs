using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Contracts.TermsConditions;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FleetM360_WebApi.Controllers
{
    // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DriverAPIController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IEmployeeService _employeeService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITripService _tripService;
        private readonly ILogger<DriverAPIController> _logger;
        private readonly ITruckService _truckService;
        private readonly IConfiguration _configuration;
        private readonly ITermsConditionsService _termsConditionsService;
        private readonly IRepository<Truck, long> _truckRepository;

        public DriverAPIController(IDriverService driverService, IEmployeeService employeeService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<DriverAPIController> logger,
            ITruckService truckService, IConfiguration configuration, ITermsConditionsService termsConditionsService, IRepository<Truck, long> truckRepository)
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
            _truckRepository = truckRepository;
        }
        [HttpPost("userLogin")]
        public async Task<ActionResult> UserLogin([Bind(include: "DriverNumber,Password")] LoginModel loginModel)
        {
            TokenModel tokenModel = new TokenModel();
            UserData userData = new UserData();
            var test = _driverService.GetAllDrivers();
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, loginModel.Password, true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, aspNetUser.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                        var refreshToken = await _employeeService.GenerateRefreshToken();

                        var Accesstoken = await _employeeService.GenerateAccessToken(authClaims);
                        if (Accesstoken != null)
                        {
                            tokenModel = new TokenModel()
                            {
                                AccessToken = new JwtSecurityTokenHandler().WriteToken(Accesstoken),
                                RefreshToken = refreshToken,
                            };
                        }
                        else
                        {
                            return BadRequest(new { Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                        }
                        aspNetUser.RefreshToken = refreshToken;
                        _ = int.TryParse(_configuration["JWT:RefreshTokenExpirationDays"], out int refreshTokenValidityInDays);
                        aspNetUser.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInDays);   //expiration date
                        var userUpdateRefreshTokenResult = await _userManager.UpdateAsync(aspNetUser);
                        //End Token 
                        if (userUpdateRefreshTokenResult.Succeeded)
                        {
                            userData.UserNumber = driver.DriverNumber;
                            if (driver.ConditionsAccept != null)
                            {
                                userData.termsAndConditionsAccept = (bool)driver.ConditionsAccept;
                            }
                            else
                            {
                                userData.termsAndConditionsAccept =false;
                            }
                                userData.TokenModel = tokenModel;
                            return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = userData });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                        }
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
                    }
                }
            }
            return BadRequest(new { flag = false, Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
        }


        [HttpPost("userLoginHomeData")]
        public async Task<ActionResult> userLoginHomeData([Bind(include: "DriverNumber")] LoginModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(aspNetUser.Email, loginModel.Password, true, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        var groupedTrips = await _tripService.GetAllPendingTripofParentTrip();//.GetAllpendingTripGroupedByParentTrip();

                        return Ok(new { Data = groupedTrips, Message = "Successful Process" });
                    }
                }
            }
            return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
        }

        [HttpPost("activeTripsSummary")] 
        public async Task<ActionResult> activeTripsSummary([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                    var truck = await _truckRepository.Find(e => e.IsVisible == true && e.Id == Convert.ToInt64(loginModel.truckId)).FirstOrDefaultAsync();
                    //if (truck != null)
                    //{ 
                    //    if(truck.status== "Maintainance")
                    //    {
                    //        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 }); // FailedAccount
                    //    }
                    //}
                    HomeDataModel homeData = new HomeDataModel();
                    homeData.driver = new userApiModel();
                    homeData.driver.userPhoneNumber = driver.PhoneNumber;
                    homeData.driver.userNumber = driver.DriverNumber.ToString();
                    homeData.driver.userName = driver.FullName;

                    //var groupedTrips = await _tripService.GetAllPendingTripofParentTrip();//.GetAllpendingTripGroupedByParentTrip();
                    homeData.trips = await _tripService.GetAllPendingTripofTruckforMobile(loginModel.truckId, loginModel.languageId);

                    // return Ok(new { Data = homeData, Message = "Successful Process" });
                    return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = homeData });

                }
            }
            //return BadRequest(new { Data = 0, Message = "رقم المستخدم أو كلمة السر خطأ" });
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [HttpPost("activetripDetails")]
        public async Task<ActionResult> activetripDetails([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                   
                    var homeData = await _tripService.GetTripDetailsForMobile(Convert.ToInt64(loginModel.tripId),loginModel.languageId);//.GetAllpendingTripGroupedByParentTrip();
                    if (homeData != null)
                    {
                        //return Ok(new { Data = groupedTrips, Message = "Successful Process" });
                        return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = homeData });
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
    }
}
