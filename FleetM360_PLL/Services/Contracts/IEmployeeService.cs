using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IEmployeeService
    {
        List<EmployeeModel> GetAllAdmins();
        Task<bool> CreateAdmin(EmployeeModel model);
        Task<bool> AcceptCondition(EmployeeModel model);
        //Task<bool> UpdateDriver(DriverModel model);
        //bool DeleteDriver(long id);
        EmployeeModel GetAdmin(long id);
        public Task<JwtSecurityToken> GenerateAccessToken(List<Claim> authClaims);

        public Task<string> GenerateRefreshToken();
    }
}
