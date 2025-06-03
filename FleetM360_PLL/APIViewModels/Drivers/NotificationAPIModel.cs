using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.APIViewModels.Drivers
{
    public class NotificationAPIModel
    {
        public int? notificationId { get; set; }
        public string? notificationTitle { get; set; }
        public string? notificationDescription { get; set; }
        public DateTime? notificationTime { get; set; }
        public bool? notificationStatus {  get; set; }
    }
    public class NotificationUserModel
    {
        public string truckId { get; set; }
        public int languageId { get; set; }
    }
}
