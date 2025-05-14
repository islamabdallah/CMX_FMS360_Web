using FleetM360_PLL.Services.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace FleetM360_Web.hub
{
    public class TruckHub : Hub
    {
        private readonly ITruckService _truckService;

        public TruckHub(ITruckService truckService)
        {
            _truckService = truckService;
        }

        public async Task SendTruckLocation(string truckId, double latitude, double longitude)
        {
            await Clients.All.SendAsync("ReceiveTruckLocation", truckId, latitude, longitude);
        }
    }
}
