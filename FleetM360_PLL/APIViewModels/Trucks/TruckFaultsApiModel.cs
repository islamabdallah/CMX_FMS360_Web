using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trucks
{
    public class TruckFaultsApiModel
    {
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public int? severityLevel { get; set; }
    }
    public class TruckFaultsDataModel
    {
        public DateTime? startTime { get; set; }
        public List<TruckFaultsApiModel> truckFaults { get; set; }
    }
    public class loadingCommentApiModel
    {
        public long userNumber { get; set; }
        public int languageId { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public string? loadingDriverComment { get; set; }
    }
}
