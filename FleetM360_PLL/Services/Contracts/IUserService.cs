using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(string username, string email, string password);
        Task<bool> AssignRoleToUserAsync(string username, string role);
        Task<bool> AddRole(string RoleName);
    }
}
