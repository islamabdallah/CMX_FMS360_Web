using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class PlannedTripLocationProduct : EntityWithIdentityId<long>
    {
        public long PlannedTripLocationId { get; set; }
        public long? MaterialId { get; set; }

        public Material? Material { get; set; }
        public double Qty { get; set; }
    }
}
