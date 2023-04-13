using DuyProject.API.Models;
using DuyProject.API.ViewModels;
using System.Security.Cryptography.Pkcs;

namespace DuyProject.API.Repositories
{
    public interface IChatService
    {
        Task AddMessageAsync(ChatMessage message);
        List<ChatMessage> GetMessages(string userName,string recipient);
        List<string> GetChatUsers(string userName);
        Task DeleteMessageAsync(string messageId);
    }
}
