using DuyProject.API.Models;

namespace DuyProject.API.Repositories
{
    public interface IChatService
    {
        Task AddMessageAsync(ChatMessage message);
        Task<List<ChatMessage>> GetMessagesAsync(string conversationId);
        Task DeleteMessageAsync(string messageId);
    }
}
