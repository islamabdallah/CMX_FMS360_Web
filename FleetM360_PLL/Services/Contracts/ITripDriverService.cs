using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface ITripDriverService
    {
        List<TripDriverModel> GetAllTripDrivers();
        Task<bool> CreateTripDriver(TripDriverModel model);
        Task<bool> UpdateTripDriver(TripDriverModel model);
        TripDriverModel GetTrip(long id);
        public Task<bool> DeleteTrip(long id);
    }
}
