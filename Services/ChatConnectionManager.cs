using DuyProject.API.Repositories;
using System.Collections.Concurrent;

namespace DuyProject.API.Services
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public void AddConnection(string userId, string connectionId)
        {
            _connections[userId] = connectionId;
        }

        public void RemoveConnection(string userId)
        {
            _connections.TryRemove(userId, out _);
        }

        public string GetConnectionId(string userId)
        {
            _connections.TryGetValue(userId, out string connectionId);
            return connectionId;
        }
    }
}
