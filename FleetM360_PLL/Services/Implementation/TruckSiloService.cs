using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class TruckSiloService : ITruckSiloService
    {
        private readonly IRepository<TruckSilo, long> _repository;
        private readonly ILogger<TruckSiloService> _logger;
        private readonly IMapper _mapper;

        public TruckSiloService(IRepository<TruckSilo, long> repository,
          ILogger<TruckSiloService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public TruckSilo GetLastActiveTruckSilo(string truckNumber)
        {
            try
            {
                TruckSilo truckSilo = _repository.Find(t => t.IsVisible == true && t.IsActive && t.TruckNumber == truckNumber).FirstOrDefault();
                return truckSilo;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
        }
    }
}
