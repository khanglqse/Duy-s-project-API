using DuyProject.API.ViewModels.File;

namespace DuyProject.API.ViewModels
{
    public class LoginViewModel
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public LoginUserViewModel User { get; set; }
    }

    public class LoginUserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }    
        public bool IsCreateBySocialAccount { get; set; }
        public List<string> Roles { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public FileViewModel? Avatar { get; set; }
    }

}
