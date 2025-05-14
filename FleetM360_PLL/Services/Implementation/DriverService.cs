using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IRepository<Driver, long> _repository;
        private readonly ILogger<DriverService> _logger;
        private readonly IMapper _mapper;

        public DriverService(IRepository<Driver, long> repository,
          ILogger<DriverService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<bool> AcceptCondition(DriverModel model)
        {
            model.ConditionsAccept = true;
            var employee = _mapper.Map<Driver>(model);
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
            //throw new NotImplementedException();
        }

        public Task<bool> CreateDriver(DriverModel model)
        {
            try
            {
                model.IsVisible = true;
                model.IsDelted = false;
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                var driver = _mapper.Map<Driver>(model);
                var result = _repository.Add(driver);

                if (result != null)
                {
                    return Task<bool>.FromResult<bool>(true);
                }
                else
                {
                    return Task<bool>.FromResult<bool>(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Task<bool>.FromResult<bool>(false);

            }
        }

        public List<DriverModel> GetAllDrivers()
        {
            try
            {
                var drivers = _repository.Findlist().Result;
                var models = new List<DriverModel>();
                models = _mapper.Map<List<DriverModel>>(drivers);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }

        public DriverModel GetDriver(long id)
        {
            try
            {
                Driver driver = _repository.Find(d => d.IsVisible == true && d.DriverNumber == id).First();
                DriverModel driverModel = _mapper.Map<DriverModel>(driver);
                return driverModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }
    }
}
