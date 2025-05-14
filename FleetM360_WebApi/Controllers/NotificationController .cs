using FleetM360_DAL.Models.MasterModels;
using FleetM360_DAL.Repository.EntityFramework;
using FleetM360_PLL;
using FleetM360_PLL.APIViewModels.Drivers;
using FleetM360_PLL.APIViewModels.Socket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FleetM360_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly WebSocketService _wsService;
        private readonly ApplicationDBContext _context;

        public NotificationController(WebSocketService wsService, ApplicationDBContext context)
        {
            _wsService = wsService;
            _context = context;
        }

        [HttpPost("{userNumber}/{truckId}/{tripId}")]
        public async Task<IActionResult> SendNotification(int userNumber, string truckId, string tripId, SocketMessageApiModel message)
        {
            string name = "";
            string StageAR = "قيد الفحص";
            string StageEn = "Under Inspection";
            if (message != null)
            {
                
                var trip = _context.Trips.Where(a => a.Id == Convert.ToInt64(tripId)).FirstOrDefault();
                if (trip != null)
                {
                    if (message.status == "maintenance_done")
                    {
                        name = "EndMaintainance";
                    }
                    else if (message.status == "empty_weight_started")
                    {
                        name = "EmptyWeight";
                        StageAR = "قيد التحميل";
                        StageEn = "Loading";
                    }
                    else if (message.status == "empty_weight_ended")
                    {
                        name = "EmptyWeight";
                        StageAR = "قيد التحميل";
                        StageEn = "Loading";
                    }
                    else if (message.status == "gross_weight_started")
                    {
                        name = "GrossWeight";
                        StageAR = "قيد التحميل";
                        StageEn = "Loading";
                    }
                    else if (message.status == "gross_weight_ended")
                    {
                        name = "GrossWeight";
                        // StageAR = "قيد التحميل";
                        // StageEn = "Loading";
                        trip.Qty=Convert.ToInt64(message.Weight_value);
                    }
                    else if (message.status == "replaced")
                    {
                        long newTruckId = 2;
                        long siloId = 4;
                        var truck = _context.Trucks.Where(a => a.Id == newTruckId).FirstOrDefault();
                        var silo = _context.Trucks.Where(a => a.Id == siloId).FirstOrDefault();
                        name = "Replaced";
                        var trips = _context.Trips.Where(a => a.ParentTrip == trip.ParentTrip).ToList();
                        if (trips != null)
                        {
                            if(trips.Count > 0)
                            {
                                foreach (var tr in trips)
                                {
                                    tr.TruckNumber = truck.TruckNumber;
                                    tr.SiloNumber = silo.TruckNumber;
                                    tr.IsConverted = true;
                                    tr.UpdatedDate = DateTime.Now;
                                    _context.Trips.Update(tr);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        // StageAR = "قيد التحميل";
                        // StageEn = "Loading";
                    }
                    var Event = _context.LogLookups.Where(t => t.IsVisible == true && t.LogName == name.Trim()).FirstOrDefault();
                    if (Event != null)
                    {
                        TripLog tripLog = new TripLog()
                        {
                            ParentTrip = trip.ParentTrip,
                            TripNumber = trip.TripNumber,
                            Event = Event.LogName,
                            LogId = Event.Id,
                           // Lat = model.lat,
                           // Long = model.lng,
                            CreatedBy ="sap",// model.UserNumber.ToString(),
                            Date = DateTime.Now.ToString(),
                            CreatedDate = DateTime.Now,
                            UpdatedDate = DateTime.Now,
                            IsDelted = false,
                            IsVisible = true
                        };
                        _context.TripLogs.Add(tripLog);
                        await _context.SaveChangesAsync();

                        //return Ok(new { flag = true, Message = UserMessage.SuccessfulProcess[model.languageId], Data = "" });
                    }
                    trip.StageAR = StageAR;
                    trip.StageEn = StageEn;
                    trip.UpdatedDate = DateTime.Now;
                    _context.Trips.Update(trip);
                    await _context.SaveChangesAsync();

                    await _wsService.SendMessageToUserAsync(userNumber, truckId, JsonSerializer.Serialize(message));
                    return Ok("Notification sent.");
                }
                else
                {
                    return BadRequest("");
                }
            }
            else
            {
                return BadRequest("");
            }
        }
    }
}
