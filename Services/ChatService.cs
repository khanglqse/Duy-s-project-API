using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.Repositories;
using MongoDB.Driver;

namespace DuyProject.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<ChatMessage> _chatMessages;

        public ChatService(IMongoClient mongoClient)
        {
            IMongoDatabase? database = mongoClient.GetDatabase(AppSettings.DbName);
            _chatMessages = database.GetCollection<ChatMessage>("ChatMessages");
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _chatMessages.InsertOneAsync(message);
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(string conversationId)
        {
            FilterDefinition<ChatMessage>? filter = Builders<ChatMessage>.Filter.Eq(x => x.ConversationId, conversationId);
            return await _chatMessages.Find(filter).ToListAsync();
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            await _chatMessages.DeleteOneAsync(m => m.Id == messageId);
        }
    }
}
