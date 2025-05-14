using FleetM360_DAL.Models.Entity;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripQuestion : EntityWithIdentityId<long>
    {
        public long PlannedTripLocationId { get; set; }     
        public PlannedTripLocation PlannedTripLocation { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }//Sap Number      
        public double Lat { get; set; }
        public double Long { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }      
        public bool Answer { get; set; }
    }
}
