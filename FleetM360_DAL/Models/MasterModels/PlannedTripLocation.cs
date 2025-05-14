using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class PlannedTripLocation : EntityWithIdentityId<long>
    {
        public long? JobSiteId { get; set; }

        public JobSite? JobSite { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public string Type { get; set; } // Source/ Destination
        public string Location { get; set; } //Location Name
        public double Lat { get; set; }
        public double Long { get; set; }//Sap Number
        public string Material { get; set; }
        public double Qty { get; set; }
    }
}
