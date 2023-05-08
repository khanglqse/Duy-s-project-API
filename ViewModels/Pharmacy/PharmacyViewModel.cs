using DuyProject.API.ViewModels.File;

namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Location Location { get; set; }
        public List<string> DrugIds { get; set; }
        public List<string> DoctorIds { get; set; }
        public List<Models.Drug> Drugs { get; set; } = new();
        public List<Models.User> Doctor { get; set; } = new();
        public string Phone { get; set; } = string.Empty;
        public string LogoId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> FollowUser { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public FileViewModel? Avatar { get; set; }
    }
}
