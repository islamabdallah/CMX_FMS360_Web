using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripPrecheckDetails : EntityWithIdentityId<long>
    {
        public long TripPrecheckId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
