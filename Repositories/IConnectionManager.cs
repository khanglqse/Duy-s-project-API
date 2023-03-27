namespace DuyProject.API.Repositories
{
    public interface IConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId);
        string GetConnectionId(string userId);
    }
}
