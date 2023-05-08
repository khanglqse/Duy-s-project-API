using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels.User
{
    public class UserUpdateCommand
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Location Location { get; set; }
        public string Locale {get;set;} = string.Empty;
        public bool IsCreateBySocialAccount { get; set; } = false;
        public string Roles { get; set; } = AppSettings.Patient;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
