using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface ITruckService
    {
        List<TruckModel> GetAllTrucks();
        Task<List<TruckModel>> GetAllTrucksWithNoMobileAsync();

        Task<bool> CreateTruck(TruckModel model);
        Task<bool> UpdateTruck(TruckModel model);
        bool DeleteTruck(string id);
        TruckModel GetTruck(long id);
        TruckModel GetPendingTruck(long id);
        TruckModel GetTruckByNumber(string truckNumber);
        List<TruckModel> GetAllActiveTrucks();

    }
}
