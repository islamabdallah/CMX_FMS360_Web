using FleetM360_DAL.Models.Entity;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripDanger : EntityWithIdentityId<long>
    {
        
        public long PlannedTripLocationId { get; set; }       
        public PlannedTripLocation PlannedTripLocation { get; set; }
        public long ParentTrip { get; set; }
        public long TripNumber { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public string MeasureControl { get; set; }
        public int DangerId { get; set; }
    }
}
