using FleetM360_PLL.Services.Contracts;
using FirebaseAdmin.Messaging;

namespace FleetM360_PLL.Services.Implementation
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {
        public async Task<string> SendNotificationAsync(string deviceToken, string title, string body)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" }
                }
            };
            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return response; // Message ID
            }
            catch (FirebaseMessagingException ex)
            {
                // Handle exception (e.g., log it)
                throw new Exception("Error sending notification", ex);
            }

            //return await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
    }
}
