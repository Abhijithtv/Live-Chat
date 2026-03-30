using ChatServer.DTO;
using ChatServer.EventHandlers;
using System.Net.WebSockets;
using System.Text;

namespace ChatServer.SocketHandler
{
    public class WebSocketProcessor(ConnectionManager connectionManager, UserSentEvent userSentEvent)
    {
        internal async Task Handle(Guid userId, WebSocket socket)
        {
            connectionManager.AddConnection(userId, socket);

            while (socket.State == WebSocketState.Open)
            {
                var serializedMsg = await ReceiveFullMessage(userId, socket);

                var msg = System.Text.Json.JsonSerializer.Deserialize<ChatMessageDTO>(serializedMsg);

                await userSentEvent.HandleMessage(userId, msg);
            }
        }

        private async Task<string?> ReceiveFullMessage(Guid userId, WebSocket socket, CancellationToken ct = default)
        {
            var buffer = new byte[4 * 1024];
            using var ms = new MemoryStream();


            while (true)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    ct
                );

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    connectionManager.RemoveConnection(userId);
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by client",
                        ct
                    );
                    return null;
                }

                if (result.MessageType != WebSocketMessageType.Text)
                {
                    continue;
                }

                ms.Write(buffer, 0, result.Count);

                if (result.EndOfMessage)
                {
                    break;
                }
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
