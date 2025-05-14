using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Implementation
{
    public class PlannedTripLocationService : IPlannedTripLocationService
    {
        public Task<bool> CreateTripLocation(PlannedTripLocation model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTrip(long id)
        {
            throw new NotImplementedException();
        }

        public List<PlannedTripLocation> GetAllTripLocations()
        {
            throw new NotImplementedException();
        }

        public PlannedTripLocation GetTrip(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTripLocation(PlannedTripLocation model)
        {
            throw new NotImplementedException();
        }
    }
}
