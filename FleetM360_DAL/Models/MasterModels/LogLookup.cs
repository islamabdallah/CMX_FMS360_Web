using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class LogLookup : EntityWithIdentityId<long>
    {
        public string LogName { get; set; }
        public string CurrentScreen { get; set; }//Sap Number
        public string NextScreen { get; set; }
        public string Status { get; set; }
        public string StatusForMobileEN { get; set; }
        public string StatusForMobileAR { get; set; }
        public string MobileKey { get; set; } //userId
    }
}
