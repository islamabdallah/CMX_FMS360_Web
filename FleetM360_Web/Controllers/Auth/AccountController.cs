using FleetM360_DAL.Models;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.Message;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.Services.Implementation;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FleetM360_Web.Controllers.Auth
{
    
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        private readonly SignInManager<ApplicationUser> SignInManger;
        private readonly UserManager<ApplicationUser> UserManger;
        private readonly IEmployeeService _employeeService;
        public ApplicationDBContext _context;

        public AccountController(IUserService userService, UserManager<ApplicationUser> userManager,
            IEmployeeService employeeService, SignInManager<ApplicationUser> signInManger, ApplicationDBContext context)
        {
            _userService = userService;
            UserManger = userManager;
            SignInManger = signInManger;
            _employeeService = employeeService;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()// LoginViewModel loginViewModel)
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
                return View(loginViewModel);
            EmployeeModel employee = _employeeService.GetAdmin(loginViewModel.CemexId);
            if (employee != null)
            {
                ApplicationUser aspNetUser = await UserManger.FindByIdAsync(employee.UserId);
                if (aspNetUser != null)
                {
                    var result = await SignInManger.PasswordSignInAsync(aspNetUser.Email, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded) 
                    {
                        return RedirectToAction("Index", "Home");
                    }
                   
                }
            }
            ModelState.AddModelError(string.Empty, "CemexId or Password Not Correct");
            var em = new LoginViewModel();
            return View(em);
        }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Employee Number is required")]
        public long CemexId { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
