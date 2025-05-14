using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class ActualTripLocation : EntityWithIdentityId<long>
    {
        public long? PlannedTripLocationId { get; set; }

        public PlannedTripLocation? PlannedTripLocation { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public long TripLogId { get; set; }
        public string Type { get; set; } // Source/ Destination
        public string? Location { get; set; } //Location Name
        public double Lat { get; set; }
        public double Long { get; set; }//Sap Number
        public string Material { get; set; }
        public double Qty { get; set; }
        public double Received { get; set; }
        public double Remain { get; set; }
        public int? Hours { get; set; }
        public int? Minutes { get; set; }
        public int? Seconds { get; set; }
    }
}
