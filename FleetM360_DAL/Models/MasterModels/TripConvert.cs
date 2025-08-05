using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripConvert : EntityWithIdentityId<long>
    {
        public long TripId { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public string createdby { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}
