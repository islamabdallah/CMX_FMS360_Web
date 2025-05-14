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

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Desc { get; set; }
        public string? City { get; set; }

        public bool HasNetworkCoverage { get; set; }
    }
}
