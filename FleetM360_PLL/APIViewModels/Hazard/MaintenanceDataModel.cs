using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Hazard
{
    public class MaintenanceDataModel
    {
        public List<CauseOfTruckFailureModel> causesOfTruckFailure { get; set; }
        public List<WayToDealWithTruckBreakdownsModel> waysToDealWithTruckBreakdowns { get; set; }
    }
    public class CauseOfTruckFailureModel
    {
        public string? id { get; set; }
        public string? name { get; set; }

    }
    public class WayToDealWithTruckBreakdownsModel
    {
        public string? id { set; get; }
        public string? name { set; get; }
    }
    public class sendStopStartTime
    {
        public int userNumber { get; set; }
        public int languageId { get; set; }
        public string causeOfFailure { get; set; }//else for stop option
        public string? wayOfDeal { get; set; }
        public string driverComment { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string truckId { get; set; }
        public string tripId { get; set; }
        public string? type { set; get; }
        public DateTime? startTime { set; get; }
        public DateTime? endTime { set; get; }
    }

    public class StopModel
    {
        public string? causeOfFailure { set; get; }
        public string? wayOfDeal { set; get; }
        public string? driverComment { set; get; }
        public double? lat { set; get; }
        public double? lng { set; get; }
        public DateTime? startTime { set; get; }
        public string? type { set; get; }
    }
}
