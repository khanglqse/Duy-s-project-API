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
            var conversationId = Guid.NewGuid().ToString();
            var conversationFirstCheck = _chatMessages.AsQueryable().Any(x=>x.Sender == message.Sender && x.Recipient == message.Recipient);
            var conversationSecondCheck = _chatMessages.AsQueryable().Any(x => x.Sender == message.Recipient && x.Recipient == message.Sender);

            if(conversationFirstCheck && conversationSecondCheck)
            {
                conversationId = _chatMessages.AsQueryable().First(x=>x.Sender == message.Sender && x.Recipient == message.Recipient).ConversationId;
            }
            if(!conversationFirstCheck && conversationSecondCheck) 
            {
                conversationId = _chatMessages.AsQueryable().First(x => x.Sender == message.Recipient && x.Recipient == message.Sender).ConversationId;
            }
            if(conversationFirstCheck && !conversationSecondCheck)
            {
                conversationId = _chatMessages.AsQueryable().First(x => x.Sender == message.Sender && x.Recipient == message.Recipient).ConversationId;
            }

            message.ConversationId = conversationId;
                
            await _chatMessages.InsertOneAsync(message);
        }

        public List<ChatMessage> GetMessages(string userName)
        {
            var chatHistory = _chatMessages.AsQueryable().Where(x=>x.Sender == userName || x.Recipient == userName).OrderBy(x=>x.Timestamp);
            return chatHistory.ToList();
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            await _chatMessages.DeleteOneAsync(m => m.Id == messageId);
        }
    }
}
