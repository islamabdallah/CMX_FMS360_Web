using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Models.MasterModels.HazardData;
using FleetM360_PLL.APIViewModels.Hazard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IRiskService
    {
        Task<int> AddDriverFeedbackAsync(DriverFeedback driverFeedback);
        Task<IEnumerable<RiskBusinessModel>> GetAllAsync();
        Task<IEnumerable<RiskLevel>> GetAllRiskLevelAsync();
        Task<int> AddAsync(Risk risk);
       // Task<IEnumerable<Countries>> CountryList();

       // Task<IEnumerable<string>> DestinationList(ApiTemplate template);

        Task<IEnumerable<Risk>> GetAllAsyncByCountry(ApiTemplate template);

        Task<IEnumerable<JobsiteHazard>> GetAllAsyncByJobsite(ApiTemplate template);

        Task<IEnumerable<JobsiteHazard>> JobsiteList(ApiTemplate template);

        Task<string> SupportByCountry(ApiTemplate template);
        Task<IEnumerable<string>> GetCheckPoints();
        //Task<int> AddShipmentTracking(ShipmentCheckPoint shipment);
       // Task<IEnumerable<CheckPointUser>> UserLogin(CheckPointUser user);

       // Task<IEnumerable<Stamp>> StampData();
    }
}
