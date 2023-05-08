namespace DuyProject.API.ViewModels.User
{
    public class GoogleLoginCommand
    {
        public string IdToken { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Location Location { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
