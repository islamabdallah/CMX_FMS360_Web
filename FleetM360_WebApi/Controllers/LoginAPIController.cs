using FleetM360_DAL.Models;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts.TermsConditions;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using FleetM360_PLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_DAL.Models.MasterModels;

namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAPIController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly IEmployeeService _employeeService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITripService _tripService;
        private readonly ILogger<LoginAPIController> _logger;
        private readonly ITruckService _truckService;
        private readonly IConfiguration _configuration;
        private readonly ITermsConditionsService _termsConditionsService;
        private readonly ApplicationDBContext _context;
        public LoginAPIController(IDriverService driverService,
            IEmployeeService employeeService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITripService tripService,
            ILogger<LoginAPIController> logger,
            ITruckService truckService,
            IConfiguration configuration, ITermsConditionsService termsConditionsService, ApplicationDBContext context)
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
        }

        [HttpPost("adminLogin")]
        public async Task<ActionResult> AdminLogin([Bind(include: "UserNumber,Password")] LoginModel loginModel)
        {
            TokenModel tokenModel = new TokenModel();
            UserData userData = new UserData();
            EmployeeModel employee = _employeeService.GetAdmin(loginModel.UserNumber);
            if (employee != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
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
                            userData.UserNumber = employee.EmployeeNumber;
                            userData.termsAndConditionsAccept = (bool)employee.ConditionsAccept;
                            userData.TokenModel = tokenModel;
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = userData });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[loginModel.languageId], Data = 0 });
                        }
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 });
                    }
                }
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginFailed[loginModel.languageId], Data = 0 });
        }

        [HttpPost("adminInfo")]
        public async Task<ActionResult> AdminInfo([Bind(include: "UserNumber")] LoginModel loginModel)
        {
            
            EmployeeModel employee = _employeeService.GetAdmin(loginModel.UserNumber);
            if (employee != null)
            {
                ApplicationUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    userApiModel employeeInfo =new userApiModel();
                    employeeInfo.userPhoneNumber = employee.PhoneNumber;
                    employeeInfo.userNumber = employee.EmployeeNumber.ToString();
                    employeeInfo.userName=employee.EmployeeName;
                    return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[loginModel.languageId], Data = employeeInfo });
                }
                return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
            }
            return BadRequest(new { flag = false, Message = UserMessage.LoginInvalidNumber[loginModel.languageId], Data = 0 });
        }

        //[AllowAnonymous]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        //[Authorize]
        [HttpGet("GetTermsOfConditions")]
        public async Task<ActionResult> GetTermsOfConditions(int languageId)
        {
            try
            {
                TermsOfConditionsModel termsOfConditionsModel = new TermsOfConditionsModel();
                if ((int)CommanData.Languages.English == languageId)
                {
                    termsOfConditionsModel = _termsConditionsService.LoadEnglishTerms().Result;
                }
                else if ((int)CommanData.Languages.Arabic == languageId)
                {
                    termsOfConditionsModel = _termsConditionsService.LoadArabicTerms().Result;
                }
                if (termsOfConditionsModel != null)
                {
                    return Ok(new { flag = true, Message = "test", Data = termsOfConditionsModel });
                    // return Ok(new { flag = true, Message = UserMessage.Done[languageId], Data = termsOfConditionsModel });
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[languageId], Data = 0 });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[languageId], Data = 0 });
            }
        }


        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AcceptConditions")]
        public async Task<ActionResult> AcceptConditions(string userNumber, int languageId , bool isAdmin)
        {
            if (isAdmin)
            {
                EmployeeModel employee = _employeeService.GetAdmin(Convert.ToInt64(userNumber));
                if (employee != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(employee.UserId);
                    if (aspNetUser != null)
                    {

                        var result2 = await _employeeService.AcceptCondition(employee);
                        if (result2)
                        {
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[languageId], Data = true });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[languageId], Data = false });
                        }


                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.InValidData[languageId], Data = false });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.InValidData[languageId], Data = false });
                }
            }
            else
            {
                //EmployeeModel employee = _employeeService.GetAdmin(Convert.ToInt64(userNumber));
                DriverModel driver = _driverService.GetDriver(Convert.ToInt64(userNumber));
                if (driver != null)
                {
                    ApplicationUser aspNetUser = await _userManager.FindByIdAsync(driver.UserId);
                    if (aspNetUser != null)
                    {

                        var result2 = await _driverService.AcceptCondition(driver);
                        if (result2)
                        {
                            return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[languageId], Data = true });
                        }
                        else
                        {
                            return BadRequest(new { flag = false, Message = UserMessage.FailedProcess[languageId], Data = false });
                        }
                    }
                    else
                    {
                        return BadRequest(new { flag = false, Message = UserMessage.InValidData[languageId], Data = false });
                    }
                }
                else
                {
                    return BadRequest(new { flag = false, Message = UserMessage.InValidData[languageId], Data = false });
                }

            }

        }


        //[Authorize]
        // [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("test")]
        public async Task<ActionResult> test(string name,string category,string mainCategory)
        {
            var Question = new PreCheckQuestion
            {
                QuestionName = name.Trim(),
                Category = category.Trim(),
                MainCategory=mainCategory.Trim(),
                IsDelted = false,
                IsVisible = true,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                PreCheckAnswers = new List<PreCheckAnswer>
                {
                    new PreCheckAnswer { AnswerNameEN = "Yes",AnswerNameAR="نعم",AnswerValue=true,IsVisible=true,IsDelted=false,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now },
                    new PreCheckAnswer { AnswerNameEN = "No",AnswerNameAR="لا",AnswerValue=false,IsVisible=true,IsDelted=false,CreatedDate=DateTime.Now,UpdatedDate=DateTime.Now }
                }
            };

            _context.PreCheckQuestions.Add(Question);
            await _context.SaveChangesAsync();

            //return Ok(author);
            return Ok(new { flag = true, Message = "Done Done ", Data = Question });              
        }
    }
}
