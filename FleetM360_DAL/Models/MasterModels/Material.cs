using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class Material : EntityWithIdentityId<long>
    {
       
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNameAR { get; set; }
        public string Packing { get; set; }
    }
}
