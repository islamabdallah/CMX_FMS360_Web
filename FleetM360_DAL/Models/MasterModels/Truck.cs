using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class Truck : EntityWithIdentityId<long>
    {

        public string TruckNumber { get; set; }//		
        public string? SapKey { get; set; }//		
        public string Type { get; set; }
        public string Category { get; set; }
        public string LicenceNumber { get; set; }
        public string? DeviceId { get; set; }
        public DateTime? LicenceExpireDate { get; set; }
        public string? status { get; set; }
        public double? Long { get; set; }
        public double? Lat { get; set; }

        //public DateTime? LastCheck { get; set; }//		
        public string? Location { get; set; }
        public string? Model { get; set; }
        public string? Year { get; set; }
        public string? TruckManufacturer { get; set; }
        public string? Chassis { get; set; }
        public string? Engine { get; set; }
        public string? PhoneNumber { get; set; }


    }
}
