using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Trip
{
    public class WeightSapModel
    {
        public string TripNumber { get; set; }
        public string? TruckNumber { get; set; }
        public double weight { get; set; }
        public string WeightType { get; set; }
    }
    public class SapTripVM
    {
        public string TripNumber { get; set; }//Sap Number
        public string Qty { get; set; }
        public string TruckNumber { get; set; }
        public string jobsiteNumber { get; set; }
        public string? materialNumber { get; set; }
        public string? customerNumber { get; set; }
        public DateTime? departureDate { get; set; }
        public DateTime? ArrivedDate { get; set; }
    }
}
