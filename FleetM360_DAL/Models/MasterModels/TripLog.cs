using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripLog : EntityWithIdentityId<long>
    {
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string Event { get; set; }
        public long LogId { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public string? CreatedBy { get; set; } //userId
        public string? Comment { get; set; }
        public string Date { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
