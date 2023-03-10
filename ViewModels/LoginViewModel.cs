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
        public string Name { get; set; }
        public bool IsCreateBySocialAccount { get; set; }
        public List<string> Roles { get; set; }
    }

}
