using ChatCommon.DTO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace MockChatClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var userId = Guid.Parse(args[0]);
            var clientSocket = new ClientWebSocket();
            var uri = new Uri($"ws://localhost:5178/ws/chat?userId={userId.ToString()}");
            await clientSocket.ConnectAsync(uri, default);

            Console.WriteLine("Connection started - start sending your msg");

            Task.Run(async () =>
            {
                await ReadAcksAsync(clientSocket);
            });

            while (true)
            {
                var msg = Console.ReadLine();

                if ("stop".Equals(msg))
                {
                    break;
                }

                var msgObj = new GenericChatMessageDTO()
                {
                    TransactionId = Guid.NewGuid(),
                    Message = msg,
                    ToUserId = new Guid("B242BA7A-A9EF-4162-BB9D-F85613B5E28D"),
                    FromUserId = userId,
                    IsFromGroup = true
                };

                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msgObj));
                await clientSocket.SendAsync(new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    default);
                Console.WriteLine(msg + "--->Sent Successfully ");
            }
            await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Message Completed", default);
        }

        private static async Task ReadAcksAsync(ClientWebSocket clientSocket)
        {
            while (clientSocket.State == WebSocketState.Open)
            {
                var serializedMsg = await ReceiveFullMessage(clientSocket);
                Console.WriteLine(serializedMsg);
            }
        }

        private static async Task<string?> ReceiveFullMessage(ClientWebSocket socket, CancellationToken ct = default)
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
