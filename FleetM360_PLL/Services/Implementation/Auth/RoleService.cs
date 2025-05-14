using AutoMapper;
using FleetM360_DAL.Models;
using FleetM360_PLL.Services.Contracts.Auth;
using FleetM360_PLL.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation.Auth
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;


        public RoleService(UserManager<ApplicationUser> userManager,

         RoleManager<ApplicationRole> roleManager,
         IMapper mapper,
        ILogger<RoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<bool> CreateRole(RoleModel model)
        {
            try
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = model.Name;
                role.ArabicRoleName = model.ArabicRoleName;
                var response = await _roleManager.CreateAsync(role);
                return response.Succeeded;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return false;
        }

        public Task<bool> DeleteRole(RoleModel model)
        {
            throw new NotImplementedException();
        }

        public Task<List<RoleModel>> GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public Task<RoleModel> GetRole(string id)
        {
            throw new NotImplementedException();
        }

        public Task<RoleModel> GetRoleByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRole(RoleModel model)
        {
            throw new NotImplementedException();
        }
    }
}
