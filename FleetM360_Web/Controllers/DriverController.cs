using FleetM360_DAL.Models;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FleetM360_Web.Controllers
{
    public class DriverController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        public DriverController(IDriverService driverService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _driverService = driverService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _environment = environment;
        }
        public bool CheckDriverNumberIdentity(long driverId)
        {
            DriverModel driverModel = _driverService.GetDriver(driverId);
            if (driverModel != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public IActionResult Index()
        {
            try
            {
                List<DriverModel> driverModels = _driverService.GetAllDrivers();
                if (TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }
                return View(driverModels);
            }
            catch (Exception e)
            {
                return RedirectToAction("ERROR404");
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: DriverController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DriverModel model)
        {
            try
            {
                ApplicationUser aspNetUser = new ApplicationUser { Email = model.Email, UserName = model.Email };
                var result = await _userManager.CreateAsync(aspNetUser, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(aspNetUser, "Driver");
                    model.UserId = aspNetUser.Id;
                    var isDriverCreated = _driverService.CreateDriver(model).Result;
                    if (isDriverCreated == true)
                    {
                        TempData["Message"] = "Driver with number " + model.Id + " are created successfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var deleteresult = _userManager.DeleteAsync(aspNetUser);
                        if (deleteresult.IsCompletedSuccessfully)
                        {
                            TempData["Error"] = "Failed to add new driver";
                        }
                        else
                        {
                            TempData["Error"] = "there is an fatal error, please contact your technical support";
                        }
                        return View(model);
                    }
                }
                else
                {
                    TempData["Error"] = "Failed to add new user";
                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("ERROR404");

            }
        }
    }
}
