using FleetM360_DAL.Models.MasterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IPlannedTripLocationService
    {
        List<PlannedTripLocation> GetAllTripLocations();
        Task<bool> CreateTripLocation(PlannedTripLocation model);
        Task<bool> UpdateTripLocation(PlannedTripLocation model);
        PlannedTripLocation GetTrip(long id);
        public Task<bool> DeleteTrip(long id);
    }
}
