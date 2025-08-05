using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services.Contracts
{
    public interface IFirebaseNotificationService
    {
        public FirebaseAdmin.Messaging.Message CreateNotification(string title, string notificationBody, string token);
        Task<string> SendNotificationAsync(string deviceToken, string title, string body);
    }
}
