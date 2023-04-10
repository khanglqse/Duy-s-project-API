using DuyProject.API.Models;
using System.Security.Cryptography.Pkcs;

namespace DuyProject.API.Repositories
{
    public interface IChatService
    {
        Task AddMessageAsync(ChatMessage message);
        List<ChatMessage> GetMessages(string userName,string recipient);
        Task DeleteMessageAsync(string messageId);
    }
}
