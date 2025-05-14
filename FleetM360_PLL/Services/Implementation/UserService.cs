using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models;
using FleetM360_PLL.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password)
        {
            var user = new ApplicationUser { UserName = username, Email = email };
            var result = await _userRepository.CreateUserAsync(user, password);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleToUserAsync(string username, string role)
        {
            var user = await _userRepository.FindByNameAsync(username);
            if (user == null) return false;

            if (await _userRepository.RoleExistsAsync(role))
            {
                var result = await _userRepository.AddToRoleAsync(user, role);
                return result.Succeeded;
            }

            return false;
        }

        public Task<bool> AddRole(string RoleName)
        {
            throw new NotImplementedException();
        }
    }
}
