using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Contracts.TermsConditions;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        private readonly ApplicationDBContext _context;

        public DriverAPIController(IDriverService driverService, IEmployeeService employeeService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<DriverAPIController> logger, ApplicationDBContext context,
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
            _context = context;
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

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
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

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
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
                    if (truck == null)
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 }); // FailedAccount                       
                    }
                    HomeDataModel homeData = new HomeDataModel();
                    bool hasActiveTrip = false;
                    homeData.driver = new userApiModel();
                    homeData.driver.userPhoneNumber = driver.PhoneNumber;
                    homeData.driver.userNumber = driver.DriverNumber.ToString();
                    homeData.driver.userName = driver.FullName;
                    homeData.trips = await _tripService.GetAllPendingTripofTruckforMobile(loginModel.truckId, loginModel.languageId);
                    homeData.screen = "";
                    if(homeData.trips !=null)
                    {
                        if (homeData.trips.Count > 0)
                        {
                            if (homeData.trips[0].subTrips !=null)
                            {
                                if (homeData.trips[0].subTrips.Count > 0)
                                {
                                    foreach(var trip in homeData.trips[0].subTrips)
                                    {
                                        if (trip.start == 1)
                                        {
                                            hasActiveTrip = true;
                                            break;
                                        }
                                    }
                                    var last = await _context.TripLogs.Where(t => t.IsVisible == true && t.ParentTrip == Convert.ToInt64(homeData.trips[0].tripId)).OrderBy(t=>t.Id).LastOrDefaultAsync();
                                    if (hasActiveTrip==false)
                                    {
                                        if(last != null)
                                        {
                                            if(last.Event== "Maintainance" || truck.status== "Maintainance")
                                            {
                                                homeData.screen = "PlantMaintenanceScreen";
                                            }
                                            else
                                            {
                                                homeData.screen = "SplashWidget";
                                                homeData.plant = new LocationApiModel()
                                                {
                                                    address = "مصنع اسمنت اسيوط",
                                                    lat = 27.179130902288716,
                                                    lng = 31.022034339860536
                                                };
                                                var arrived= await _context.TripLogs.Where(t => t.IsVisible == true && t.ParentTrip == Convert.ToInt64(homeData.trips[0].tripId) && t.Event== "AutomaticArrivePlant").FirstOrDefaultAsync();
                                                homeData.plant.arrivalFlag=arrived !=null? true : false;
                                            }
                                        }

                                    }

                                }
                            }
                            
                        }
                    }
                    var userNotificationModels = await _context.TruckNotifications.Where(UN => UN.TruckNumber == truck.TruckNumber && UN.Seen==false).ToListAsync();
                    homeData.userUnSeenNotificationCount = userNotificationModels !=null?userNotificationModels.Count : 0;
                    if (truck.status == "Maintainance")
                    {
                        homeData.screen = "PlantMaintenanceScreen";
                    }
                    return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = homeData });

                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 }); // FailedAccount
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("activetripDetails")]
        public async Task<ActionResult> activetripDetails([Bind(include: "DriverNumber")] UserApiModel loginModel)
        {
            DriverModel driver = _driverService.GetDriver(loginModel.UserNumber);
            if (driver != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                if (aspNetUser != null)
                {
                   
                    var homeData = await _tripService.GetTripDetailsForMobile(Convert.ToInt64(loginModel.tripId),loginModel.languageId);
                    if (homeData != null)
                    {
                        return Ok(new { flag = true, Message = UserMessage.Done[loginModel.languageId], Data = homeData });
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
        [HttpPost("mobileToken")]
        public async Task<ActionResult> UpdateToken(string userNumber, string newToken, string languageId)
        {
            DriverModel driver = _driverService.GetDriver(Convert.ToInt64(userNumber));
          
                bool result = false;
            if (driver != null)
            {
                //EmployeeModel employeeModel = _EmployeeService.GetEmployeeByUserId(userId).Result;
                driver.MobileToken = newToken;
                result = _driverService.UpdateDriver(driver).Result;
            }
            if (result == true)
            {
                return Ok(new { Message = UserMessage.SuccessfulProcess[Convert.ToInt32(languageId)], Data = true });
            }
            else
            {
                return BadRequest(new { Message = UserMessage.InvalidEmployeeData[Convert.ToInt32(languageId)], Data = false });
            }
        }

        //[AllowAnonymous]
        [HttpPost("refreshTokenOld")]
        public async Task<IActionResult> RefreshTokenOld(TokenModel tokenModel, int languageId)
        {
            if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.AccessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken)) //(tokenModel is null)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
                // return Unauthorized(new { Message = "emptyModel", Data = 0 });
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = _employeeService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }
            else
            {
                if (principal.Result != null)
                {
                    var principalIdentity = principal.Result.Identity;
                    if (principalIdentity != null)
                    {
                        string username = principalIdentity.Name;

                        var user = await _userManager.FindByNameAsync(username);

                        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                        {
                            return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 }); ;
                        }

                        var newAccessToken = _employeeService.GenerateAccessToken(principal.Result.Claims.ToList());
                        //var newRefreshToken = GenerateRefreshToken();

                        //user.RefreshToken = newRefreshToken;
                        //await _userManager.UpdateAsync(user);

                        //var userUpdateRefreshTokenResult = await _userManager.UpdateAsync(user);
                        //End Token

                        if (newAccessToken != null)
                        {
                            TokenModel newTokenModel = new TokenModel()
                            {
                                RefreshToken = tokenModel.RefreshToken,
                                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken.Result),
                            };
                            return Ok(new { Message = UserMessage.Done[languageId], Data = newTokenModel });
                        }
                        else
                        {
                            return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
                        }
                    }
                    else
                    {
                        return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
                    }
                }
                else
                {
                    return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
                }

            }


        }


        [HttpPost("refreshTokenUpdate")]
        public async Task<IActionResult> RefreshTokenUpdate(TokenModel tokenModel, int languageId)
        {
            if (tokenModel == null || string.IsNullOrWhiteSpace(tokenModel.AccessToken) || string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            // Step 1: Try to extract the principal from the expired access token
            var principal = await _employeeService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
            if (principal == null)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            string username = principal.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            // Step 2: Find the user and validate the refresh token
            var user = await _userManager.FindByNameAsync(username);
            if (user == null ||
                user.RefreshToken != tokenModel.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            // Step 3: Generate a new access token (optionally refresh token too)
            var newAccessToken = await _employeeService.GenerateAccessToken(principal.Claims.ToList());

            if (newAccessToken == null)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            var response = new TokenModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = tokenModel.RefreshToken  // Reuse old refresh token unless you want to rotate
            };

            return Ok(new { Message = UserMessage.Done[languageId], Data = response });
        }


        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel, int languageId)
        {
            // التحقق من أن النموذج (Model) يحتوي على التوكنات
            if (tokenModel == null ||
                string.IsNullOrWhiteSpace(tokenModel.AccessToken) ||
                string.IsNullOrWhiteSpace(tokenModel.RefreshToken))
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            // استخراج بيانات المستخدم من التوكن المنتهي
            var principal = await _employeeService.GetPrincipalFromExpiredToken(accessToken);

            if (principal == null || principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            string username = principal.Identity.Name;

            // البحث عن المستخدم في قاعدة البيانات
            var user = await _userManager.FindByNameAsync(username);

            // التحقق من صلاحية الـ Refresh Token
            if (user == null ||
                user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            // توليد Access Token جديد
            var newAccessToken = _employeeService.GenerateAccessToken(principal.Claims.ToList());

            if (newAccessToken == null || newAccessToken.Result == null)
            {
                return Unauthorized(new { Message = UserMessage.WrongToken[languageId], Data = 0 });
            }

            // تجهيز نموذج التوكن الجديد للإرجاع
            var newTokenModel = new TokenModel
            {
                RefreshToken = refreshToken, // يمكنك أيضاً تجديده إذا أردت
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken.Result)
            };

            return Ok(new { Message = UserMessage.Done[languageId], Data = newTokenModel });
        }


    }
}
