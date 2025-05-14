using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TripRoadMaintenance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long userNumber { get; set; }
        public string truckId {  get; set; }
        public long tripId {  get; set; }
        public long causeOfFailureId { get; set; }
        public long wayOfDealId { get; set; }
        public long TripLogId { get; set; }   
        public string driverComment { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        [DefaultValue(false)]
        public bool IsDelted { get; set; }

        [DefaultValue(true)]
        public bool IsVisible { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
 
    }
}
