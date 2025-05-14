using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class Trip : EntityWithIdentityId<long>
    {
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number
        public string TruckNumber { get; set; }
        public string SiloNumber { get; set; }
        public long TypeId { get; set; } // (Cement/Backuling/Missions)
        public long SubTypeId { get; set; }
        public DateTime Date { get; set; }
        public long StatusId { get; set; } //(Pending/OnRoad/Completed) 
        public string StageEn { get; set; }
        public string StageAR { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsConverted { get; set; }
        public double Qty { get; set; }
        public DateTime ArrivedDate { get; set; }

        public DateTime departureDate { get; set; }


    }
}
