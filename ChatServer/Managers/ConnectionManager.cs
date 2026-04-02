using System.Net.WebSockets;

namespace ChatServer.Managers
{
    public class ConnectionManager
    {
        private readonly Dictionary<Guid, WebSocket> _connections = new Dictionary<Guid, WebSocket>();

        public WebSocket? GetConnection(Guid userId) => _connections!.GetValueOrDefault(userId, default);

        public void AddConnection(Guid userId, WebSocket socket) => _connections.Add(userId, socket);

        public void RemoveConnection(Guid userId) => _connections.Remove(userId);
    }
}
