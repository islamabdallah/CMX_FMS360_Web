using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TruckFailure : EntityWithIdentityId<long>
    {
        public long? ParentTrip { get; set; }
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public long DriverNumber { get; set; }
        public string Category { get; set; }
        public string Responsible { get; set; }
        public long? TripLogId { get; set; }
        public ICollection<TruckFailureDetail> TruckFailures { get; set; } 
    }
}
