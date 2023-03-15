using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels.User
{
    public class UserCreateCommand
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; }
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsCreateBySocialAccount { get; set; } = false;
        public string Roles { get; set; } = AppSettings.Patient;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
