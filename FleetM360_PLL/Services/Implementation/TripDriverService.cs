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
    public class TripDriverService : ITripDriverService
    {
        private readonly IRepository<TripDriver, long> _repository;
        private readonly ILogger<TripDriverService> _logger;
        private readonly IMapper _mapper;

        public TripDriverService(IRepository<TripDriver, long> repository,
        ILogger<TripDriverService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<bool> CreateTripDriver(TripDriverModel model)
        {
            try
            {
                var trip = _mapper.Map<TripDriver>(model);
                TripDriver result = _repository.Add(trip);
                if (result != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public Task<bool> DeleteTrip(long id)
        {
            throw new NotImplementedException();
        }

        public TripDriverModel GetTrip(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTripDriver(TripDriverModel model)
        {
            throw new NotImplementedException();
        }

        List<TripDriverModel> ITripDriverService.GetAllTripDrivers()
        {
            throw new NotImplementedException();
        }
    }
}
