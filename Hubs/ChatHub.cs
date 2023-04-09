using DuyProject.API.Models;
using DuyProject.API.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace DuyProject.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;
        private readonly IConnectionManager _connections;
        private readonly IChatService _chatService;

        public ChatHub(ILogger<ChatHub> logger, IConnectionManager connections, IChatService chatService)
        {
            _logger = logger;
            _connections = connections;
            _chatService = chatService;
        }

        public async Task SendMessageToUser(string conversationId, string sender, string recipient, string message, string attachmentUrl = "")
        {
            string? recipientConnectionId = _connections.GetConnectionId(recipient);
            if (recipientConnectionId != null)
            {
                await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", sender, recipient, message);
                await _chatService.AddMessageAsync(new ChatMessage
                {
                    Sender = sender,
                    Recipient = recipient,
                    Message = message,
                    Timestamp = DateTime.UtcNow,
                    AttachmentUrl = attachmentUrl,
                });
            }
        }

        public List<ChatMessage> GetMessagesHistoryForUser(string userName)
        {
            return _chatService.GetMessages(userName);
        }

        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.Identity.Name;
            _connections.AddConnection(userId, Context.ConnectionId);
            _logger.LogInformation(userId + " connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.User.Identity.Name;
            _connections.RemoveConnection(userId);
            _logger.LogInformation(userId + " disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task DeleteMessage(string messageId)
        {
            await _chatService.DeleteMessageAsync(messageId);
        }
    }
}
