using FleetM360_PLL.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FireBaseNotificationController : ControllerBase
    {
        private readonly IFirebaseNotificationService _firebaseNotificationService;

        public FireBaseNotificationController(IFirebaseNotificationService firebaseNotificationService)
        {
            _firebaseNotificationService = firebaseNotificationService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var messageId = await _firebaseNotificationService.SendNotificationAsync(
                request.DeviceToken,
                request.Title,
                request.Body);

            return Ok(new { MessageId = messageId });
        }
    }
    public class NotificationRequest
    {
        public string DeviceToken { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
