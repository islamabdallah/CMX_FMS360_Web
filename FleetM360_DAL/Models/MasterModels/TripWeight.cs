using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripWeight : EntityWithIdentityId<long>
    {
        public long TripNumber { get; set; }//Sap Number
        public long ParentTrip { get; set; }
        public string? TruckNumber { get; set; }
        public string? CreatedBy { get; set; }
        public double Weight { get; set; }
        public string Type { get; set; }
    }
}
