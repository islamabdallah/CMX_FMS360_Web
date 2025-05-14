using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TruckSilo : EntityWithIdentityId<long>
    {

        public string TruckNumber { get; set; }//		
        public string SiloNumber { get; set; }//	
        public string CreatedBy { get; set; }                        //	  [DefaultValue(false)]
        public bool IsActive { get; set; }
    }
}
