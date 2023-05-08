using DuyProject.API.ViewModels.File;

namespace DuyProject.API.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsCreateBySocialAccount { get; set; } = false;
    public Location Location { get; set; }
    public string Roles { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public FileViewModel? Avatar { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}