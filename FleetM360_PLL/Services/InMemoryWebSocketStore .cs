using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL.Services
{

    public class InMemoryWebSocketStore// : IWebSocketConnectionStore
    {
        private readonly ConcurrentDictionary<(int userNumber, string truckId), WebSocket> _connections = new();

        public void AddConnection(int userNumber, string truckId, WebSocket socket)
            => _connections[(userNumber, truckId)] = socket;

        public void RemoveConnection(int userNumber, string truckId)
            => _connections.TryRemove((userNumber, truckId), out _);

        public WebSocket? GetConnection(int userNumber, string truckId)
            => _connections.TryGetValue((userNumber, truckId), out var socket) ? socket : null;
    }
}
