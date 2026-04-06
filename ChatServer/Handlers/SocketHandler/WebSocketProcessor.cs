using ChatCommon.Constants;
using ChatCommon.DTO;
using ChatCommon.Models;
using ChatServer.EventHandlers;
using ChatServer.Managers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ChatServer.Handlers.SocketHandler
{
    public class WebSocketProcessor(ConnectionManager connectionManager, UserSentEvent userSentEvent)
    {
        internal async Task Handle(Guid userId, WebSocket socket)
        {
            connectionManager.AddConnection(userId, socket);

            while (socket.State == WebSocketState.Open)
            {
                var serializedMsg = await ReceiveFullMessage(userId, socket);
                var msg = JsonSerializer.Deserialize<GenericChatMessageDTO>(serializedMsg);

                UserSentMsgAck ack;
                try
                {
                    ack = await userSentEvent.HandleMessage(userId, msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    ack = new UserSentMsgAck()
                    {
                        TransactionId = msg.TransactionId,
                        Status = MessageStatusEnum.Failed.ToString(),
                        Msg = "Internal Server Error"
                    };
                }
                await SendAck(ack, socket, default);
            }
        }

        private async Task SendAck(UserSentMsgAck ack, WebSocket socket, CancellationToken ct)
        {
            var serializedAck = JsonSerializer.Serialize(ack);
            byte[] byteArray = Encoding.ASCII.GetBytes(serializedAck);
            await socket.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, ct);
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
