using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TruckFailureDetail : EntityWithIdentityId<long>
    {
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Date { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public long TruckFailureId { get; set; }
        public TruckFailure TruckFailure { get; set; }
    }
}
