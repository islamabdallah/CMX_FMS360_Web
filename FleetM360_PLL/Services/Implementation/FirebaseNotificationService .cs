using FleetM360_PLL.Services.Contracts;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace FleetM360_PLL.Services.Implementation
{
    public class FirebaseNotificationService : IFirebaseNotificationService
    {      
        private static bool _isInitialized = false;
        public FirebaseNotificationService()
        {
            if (!_isInitialized)
            {
                var path = Path.Combine(AppContext.BaseDirectory, "App_Data", "firebase-adminsdk.json");

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(path)
                });
                _isInitialized = true;
            }
        }
        public FirebaseAdmin.Messaging.Message CreateNotification(string title, string notificationBody, string token)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = notificationBody,
                    Title = title,
                }
            };
            return message;
        }

        public async Task<string> SendNotificationAsync(string deviceToken, string title, string body)
        {
            var message = new FirebaseAdmin.Messaging.Message()
            {
                Token = deviceToken,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return response;
        }
    }
}
