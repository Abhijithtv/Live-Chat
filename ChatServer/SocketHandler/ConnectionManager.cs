using System.Net.WebSockets;

namespace ChatServer.SocketHandler
{
    public class ConnectionManager
    {
        private Dictionary<Guid, WebSocket> _connections = new Dictionary<Guid, WebSocket>();

        public WebSocket GetConnection(Guid userId) => _connections[userId];

        public void AddConnection(Guid userId, WebSocket socket) => _connections.Add(userId, socket);

        public void RemoveConnection(Guid userId) => _connections.Remove(userId);
    }
}
