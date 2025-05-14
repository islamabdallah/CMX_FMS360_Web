using AutoMapper;
using FleetM360_DAL.Data.Repository;
using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using FleetM360_PLL.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class TruckService : ITruckService
    {
        private readonly IRepository<Truck, string> _repository;
        private readonly ILogger<TruckService> _logger;
        private readonly IMapper _mapper;

        public TruckService(IRepository<Truck, string> repository,
          ILogger<TruckService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public Task<bool> CreateTruck(TruckModel model)
        {

            try
            {
                model.CreatedDate = DateTime.Now;
                model.UpdatedDate = DateTime.Now;
                model.IsDelted = false;
                model.IsVisible = true;
                //model.Id = null;
                var truck = _mapper.Map<Truck>(model);
                var result = _repository.Add(truck);

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
            }
            return Task<bool>.FromResult<bool>(false);
        }

        public bool DeleteTruck(string id)
        {
            //try
            //{
            //    var truck = _repository.Find(t => t.Id == id).FirstOrDefault();
            //    if (truck != null)
            //    {
            //        truck.IsDelted = true;
            //        truck.IsVisible = false;
            //        bool result = _repository.Update(truck);
            //        return result;
            //    }
            //}
            //catch (Exception e)
            //{
            //    _logger.LogError(e.ToString());
            //}
            return false;
        }

        public List<TruckModel> GetAllActiveTrucks()
        {
            try
            {
                var trucks = _repository.Find(d => d.IsVisible == true).ToList();
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }

        public List<TruckModel> GetAllTrucks()
        {
            try
            {
                var trucks = _repository.Findlist().Result;
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }

        public async Task<List<TruckModel>> GetAllTrucksWithNoMobileAsync()
        {
            try
            {
                var trucks = await _repository.Find(i => i.IsVisible == true && i.Type == "Truck" && i.DeviceId == null).ToListAsync();
                // var trucks = _repository.Findlist().Result;
                var models = new List<TruckModel>();
                models = _mapper.Map<List<TruckModel>>(trucks);
                return models;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return null;
        }

        public TruckModel GetPendingTruck(long id)
        {
            try
            {
                Truck truck = _repository.Find(t => t.IsVisible == true && t.Id == id && t.DeviceId==null).First();
                TruckModel truckModel = _mapper.Map<TruckModel>(truck);
                return truckModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            //return null;
        }

        public TruckModel GetTruck(long id)
        {
            try
            {
                Truck truck = _repository.Find(t => t.IsVisible == true && t.Id == id).First();
                TruckModel truckModel = _mapper.Map<TruckModel>(truck);
                return truckModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            //return null;
        }

        public TruckModel GetTruckByNumber(string truckNumber)
        {
            try
            {
                Truck truck = _repository.Find(t => t.IsVisible == true && t.TruckNumber == truckNumber).First();
                TruckModel truckModel = _mapper.Map<TruckModel>(truck);
                return truckModel;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return null;
            }
            //return null;
        }

        public Task<bool> UpdateTruck(TruckModel model)
        {
            var truck = _mapper.Map<Truck>(model);

            try
            {
                bool result = _repository.Update(truck);

                return Task<bool>.FromResult<bool>(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Task<bool>.FromResult<bool>(false);
        }
    }
}
