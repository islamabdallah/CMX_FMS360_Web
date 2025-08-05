using FleetM360_PLL.Services.Contracts;
using FleetM360_Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FleetM360_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFirebaseNotificationService _firebaseService;

        public HomeController(ILogger<HomeController> logger, IFirebaseNotificationService firebaseService)
        {
            _logger = logger;
            _firebaseService = firebaseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Send(string deviceToken, string title, string body)
        {
            try
            {
                var result = await _firebaseService.SendNotificationAsync(deviceToken, title, body);
                ViewBag.Message = "Notification sent successfully: " + result;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
