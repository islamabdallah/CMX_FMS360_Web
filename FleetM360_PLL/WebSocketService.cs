using FleetM360_PLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace FleetM360_PLL
{
    public class WebSocketService
    {
        private readonly InMemoryWebSocketStore _connectionStore;

        public WebSocketService(InMemoryWebSocketStore store)
        {
            _connectionStore = store;
        }

        public async Task HandleConnectionAsync(int userNumber, string truckId, WebSocket socket)
        {
            _connectionStore.AddConnection(userNumber, truckId, socket);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _connectionStore.RemoveConnection(userNumber, truckId);
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                }
            }
        }

        public async Task SendMessageToUserAsync(int userNumber, string truckId, string message)
        {
            var socket = _connectionStore.GetConnection(userNumber, truckId);
            if (socket != null && socket.State == WebSocketState.Open)
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
