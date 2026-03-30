using ChatServer.SocketHandler;

namespace ChatServer.Middlewares
{
    public class ChatWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketProcessor _processor;

        public ChatWebSocketMiddleware(RequestDelegate next, WebSocketProcessor processor)
        {
            _next = next;
            _processor = processor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path != "/ws/chat")
            {
                await _next(context);
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var userId = Guid.Parse(context.Request.Query["userId"]!);
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await _processor.Handle(userId, webSocket);
        }
    }
}
