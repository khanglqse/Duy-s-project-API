using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels.User
{
    public class UserCreateCommand : UserUpdateCommand
    {
        public string Id { get; set; }
    }
}
