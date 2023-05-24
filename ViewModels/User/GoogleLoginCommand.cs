namespace DuyProject.API.ViewModels.User
{
    public class GoogleLoginCommand
    {
        public string IdToken { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Locale { get; set;}
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Type { get; set; } = "Point";
        public DateTime DoB { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
