using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class JobSite : EntityWithIdentityId<long>
    {

        public string Name { get; set; }
        public string? Number { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Desc { get; set; }
        public string? City { get; set; }

        public bool HasNetworkCoverage { get; set; }

        public string? State { get; set; }
        public string? Type { get; set; }

        public string? CustomerNumber { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhoneNumber { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhoneNumber { get; set; }
        public string? IdealKM { get; set; }
        public string? IdealTime { get; set; }
    }
}
