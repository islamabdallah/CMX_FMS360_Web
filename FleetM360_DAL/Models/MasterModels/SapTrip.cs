using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class SapTrip : EntityWithIdentityId<long>
    {
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public double Qty { get; set; }
        public string jobsiteNumber { get; set; }
        public string? materialNumber { get; set; }
        public string? customerNumber { get; set; }
        public DateTime? departureDate { get; set; }
        public DateTime? ArrivedDate { get; set; }
    }
}
