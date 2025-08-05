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
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

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

        public async Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string? token)
        {
            //try
            //{
            //    var tokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = false,
            //        ValidateIssuer = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
            //        ValidateLifetime = false
            //    };

            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            //    if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            //        return null;
            //    //throw new SecurityTokenException("Invalid token");

            //    return principal;
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}

            //try
            //{
            //    SecurityToken validatedToken;
            //    ClaimsPrincipal claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters()
            //    {
            //        ValidateAudience = false,
            //        ValidateIssuer = false,
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = (SecurityKey)new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._configuration["JWT:Key"])),
            //        ValidateLifetime = false
            //    }, out validatedToken);
            //    return !(validatedToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals("HS256", StringComparison.InvariantCultureIgnoreCase) ? (ClaimsPrincipal)null : claimsPrincipal;
            //}
            //catch (Exception ex)
            //{
            //    return (ClaimsPrincipal)null;
            //}

            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false, // لا يتم التحقق من الـ Audience
                    ValidateIssuer = false,   // لا يتم التحقق من الـ Issuer
                    ValidateIssuerSigningKey = true, // يتم التحقق من مفتاح التوقيع
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? throw new InvalidOperationException("مفتاح JWT غير موجود في الإعدادات"))
                    ),
                    ValidateLifetime = false // لا يتم التحقق من انتهاء الصلاحية
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                // التأكد من أن التوكن هو JWT وأنه يستخدم خوارزمية HMAC SHA256
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception)
            {
                // في حال حدوث أي خطأ، يتم إرجاع null
                return null;
            }

        }
    }
}
