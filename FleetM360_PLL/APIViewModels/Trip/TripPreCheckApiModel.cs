using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class TripPreCheckApiModel
    {
        public long UserNumber { get; set; }
        public int languageId { get; set; }
        public string? truckId { get; set; }
        public string? tripId { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public int category { get; set; }
        public DateTime date { get; set; }
        public List<int>? questionIds { get; set; }
    }
    public class DataInfoApiModel
    {
        public String route { get; set; }
    }
    public class TruckFaultsModel{
        public String shortDescription { get; set; }
        public String longDescription { get; set; }
        public int severityLevel { get; set; }
    }
}
