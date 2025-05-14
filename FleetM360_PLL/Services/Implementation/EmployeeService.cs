using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee, long> _repository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public EmployeeService(IRepository<Employee, long> repository,
          ILogger<EmployeeService> logger, IMapper mapper, IConfiguration configuration)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }
        public Task<bool> CreateAdmin(EmployeeModel model)
        {
            throw new NotImplementedException();
        }

        public List<EmployeeModel> GetAllAdmins()
        {
            throw new NotImplementedException();
        }

        public EmployeeModel GetAdmin(long id)
        {
            try
            {
                Employee driver = _repository.Find(d => d.IsVisible == true && d.EmployeeNumber == id).First();
                EmployeeModel driverModel = _mapper.Map<EmployeeModel>(driver);
                return driverModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public async Task<JwtSecurityToken> GenerateAccessToken(List<Claim> authClaims)
        {
            try
            {
                //var testNull = _configuration["jwtSettings:Secret"];
                //var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]);

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwtSettings:Secret"]));
                _ = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out int tokenValidityInMinutes);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return token;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public Task<bool> AcceptCondition(EmployeeModel model)
        {
            model.ConditionsAccept = true;
            var employee = _mapper.Map<Employee>(model);
            bool result = false;
            try
            {
                employee.UpdatedDate = DateTime.Now;
                result = _repository.Update(employee);

                return Task<bool>.FromResult<bool>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Task<bool>.FromResult<bool>(false);
            // throw new NotImplementedException();
        }
    }
}
