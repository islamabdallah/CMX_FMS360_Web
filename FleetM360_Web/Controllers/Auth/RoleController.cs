using FleetM360_PLL.Services.Contracts.Auth;
using FleetM360_PLL.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FleetM360_Web.Controllers.Auth
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleModel model)
        {
            try
            {
                var result = await _roleService.CreateRole(model);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

    }
}
