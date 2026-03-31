using ChatServer.SocketHandler;

namespace ChatServer.Middlewares
{
    public class ChatWebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public ChatWebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, WebSocketProcessor processor)
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
            await processor.Handle(userId, webSocket);
        }
    }
}
