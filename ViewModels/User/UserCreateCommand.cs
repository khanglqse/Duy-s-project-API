using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels.User
{
    public class UserCreateCommand
    {
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DoB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Roles { get; set; } = AppSettings.Patient;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
