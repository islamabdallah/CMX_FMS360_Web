using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IPreCheckService
    {
        Task<bool> AddTripPrecheck(TripPreCheckApiModel model);
    }
}
