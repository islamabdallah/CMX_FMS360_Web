using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripConvertLocation : EntityWithIdentityId<long>
    {
        public long TripConvertId { get; set; }
        public long OldLocId { get; set; }
        public long NewLocId { get; set; }
    }
}
