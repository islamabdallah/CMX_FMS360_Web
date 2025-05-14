using FleetM360_DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Data.Repository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindByNameAsync(string username);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> RoleExistsAsync(string roleName);
    }
}
