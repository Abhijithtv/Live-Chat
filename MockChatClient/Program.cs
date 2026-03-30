using System.Net.WebSockets;
using System.Text;

namespace MockChatClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var userId = Guid.NewGuid().ToString();
            var clientSocket = new ClientWebSocket();
            var uri = new Uri($"ws://localhost:5178/ws/chat?userId={userId.ToString()}");
            await clientSocket.ConnectAsync(uri, default);

            Console.WriteLine("Connection started - start sending your msg");

            while (true)
            {
                var msg = Console.ReadLine();

                if ("stop".Equals(msg))
                {
                    break;
                }

                var bytes = Encoding.UTF8.GetBytes(msg);
                await clientSocket.SendAsync(new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    default);
                Console.WriteLine(msg + "--->Sent Successfully ");
            }
            await clientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Message Completed", default);
        }
    }
}
