using DuyProject.API.ViewModels.File;

namespace DuyProject.API.ViewModels
{
    public class UserChatView
    {
        public IEnumerable<ChatViewModel> chatViewModels { get; set; }
    }
    public class ChatViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public FileViewModel? Avatar { get; set; }
    }
}
