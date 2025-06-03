using FleetM360_DAL.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_DAL.Models.MasterModels
{
    public class TruckNotification : EntityWithIdentityId<long>
    {
        public string TruckNumber { get; set; }
        public long? TripLogId { get; set; }
        public string? notificationTitle { get; set; }
        public string? notificationDescription { get; set; }
        public string? notificationTitleAR { get; set; }
        public string? notificationDescriptionAR { get; set; }
        public bool Seen { get; set; }
    }
}
