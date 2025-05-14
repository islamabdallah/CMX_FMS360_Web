using FleetM360_DAL.Models.MasterModels;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Trip;
using FleetM360_PLL.APIViewModels.Trucks;
using FleetM360_PLL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface ITripService
    {
        List<TripModel> GetAllTrip();
        Task<List<IGrouping<long, Trip>>> GetAllpendingTripGroupedByParentTrip();
        Task<List<TripGroupViewModel>> GetAllPendingTripofParentTrip();
        Task<List<TripApiModel>> GetAllPendingTripofTruckforMobile(string truckId,int languageId);
        Task<Trip> GetActiveParentTripOfTruck(string truckNumber);
        Task<StartTripApiModel> GetHealthPrecheck(UserApiModel model);
        Task<List<PrecheckQuestionApiModel>> GetToolsPrecheck(UserApiModel model);
        Task<List<PrecheckQuestionApiModel>> GetPrecheckListForCheck(UserApiModel model);
        Task<TruckFaultsDataModel> GettruckFaults(UserApiModel model);
        Task<bool> CreateTrip(TripModel model);
        Task<bool> UpdateTrip(TripModel model);
        Task<TripModel> GetTrip(long id);
        Task<SubTripApiModel> GetTripDetailsForMobile(long id,int languageId);
        public Task<bool> DeleteTrip(long id);
        Task<Take5APIDataModel> GetTake5DataForMobile(UserApiModel loginModel);

    }
}
