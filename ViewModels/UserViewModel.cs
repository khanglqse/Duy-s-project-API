using DuyProject.API.ViewModels.File;

namespace DuyProject.API.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; }
    public bool IsActive { get; set; }
    public bool IsCreateBySocialAccount { get; set; } = false;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Type { get; set; } = "Point";
    public double[] Coordinates { get; set; }   
    public string Roles { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public FileViewModel? Avatar { get; set; }
    public string FullName => $"{FirstName} {LastName}";
}