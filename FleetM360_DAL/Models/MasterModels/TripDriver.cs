using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripDriver : EntityWithIdentityId<long>
    {
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public long DriverId { get; set; } // Employee Id
        public string Role { get; set; } //on raod/ loading/ CooDriver
    }
}
