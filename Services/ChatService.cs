using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.Repositories;
using DuyProject.API.ViewModels;
using MongoDB.Driver;

namespace DuyProject.API.Services
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<ChatMessage> _chatMessages;
        private readonly IMongoCollection<User> _users;
        private readonly UserService _userService;
        private readonly IFileService _fileService;

        public ChatService(IMongoClient mongoClient, UserService userService, IFileService fileService)
        {
            IMongoDatabase? database = mongoClient.GetDatabase(AppSettings.DbName);
            _chatMessages = database.GetCollection<ChatMessage>("ChatMessages");
            _userService = userService;
            _fileService = fileService;
            _users = database.GetCollection<User>("User");
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

        public List<ChatMessage> GetMessages(string userName, string recipient)
        {
            var chatHistory = _chatMessages.AsQueryable().Where(x=>(x.Sender == userName || x.Recipient == userName)&& (x.Sender == recipient || x.Recipient == recipient)).OrderBy(x=>x.Timestamp);
            return chatHistory.ToList();
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            await _chatMessages.DeleteOneAsync(m => m.Id == messageId);
        }

        public async Task<ServiceResult<UserChatView>> GetChatUsers(string userName)
        {
            var user = await _users.Find(x => x.UserName == userName).FirstOrDefaultAsync();
            var chatViews = new List<ChatViewModel>();

            foreach (var connectedChatUser in user.ConnectedChatUser)
            {
                var chatViewModel = new ChatViewModel
                {
                    Id = connectedChatUser.Id,
                    UserName = connectedChatUser.UserName
                };

                try
                {
                    chatViewModel.Avatar = _fileService.ReadFileAsync(_userService.GetByUserNameAsync(connectedChatUser.UserName).Result.Data.Id).Result.Data;
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex);
                }

                chatViews.Add(chatViewModel);
            }

            var result = new UserChatView { chatViewModels = chatViews }; 

            return new ServiceResult<UserChatView>(result);
         }
    }
}
