using ChatServer.SocketHandler;
using Microsoft.AspNetCore.Mvc;

namespace ChatServer.Controllers.SocketController
{
    public class ChatController(WebSocketProcessor webSocketProcessor) : ControllerBase
    {
        [Route("/ws/chat")]
        public async Task Get([FromQuery] Guid userId)
        {
            if (userId != Guid.Empty && HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await webSocketProcessor.Handle(userId, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
