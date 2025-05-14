using FleetM360_PLL.APIViewModels.Hazard;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.APIViewModels.Trucks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface ITripLogService
    {
        Task<bool> CreateTrepFuelAsync(sendFuelDataApiModel model);
        Task<bool> CreateArriveSiteAsync(TruckStatusApiModel model);
        Task<bool> CreateStartRoadMaintenanceAsync(sendStopStartTime model);
        Task<bool> CreateSiteProcessingAsync(sendTake5DataApiModel model);
        Task<bool> CreateStartStopBanAsync(sendStopStartTime model);
        Task<bool> CreateEndRoadMaintenanceAsync(sendStopStartTime model);
        Task<bool> CreateEndStopBanAsync(sendStopStartTime model);
        Task<string> UploadedImageAsync(IFormFile ImageName, string path);
    }
}
