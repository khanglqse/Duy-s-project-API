namespace DuyProject.API.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Ward { get; set; } = string.Empty;
    public bool IsCreateBySocialAccount { get; set; } = false;
    public string Address { get; set; } = string.Empty;
    public DateTime DoB { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Roles { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}