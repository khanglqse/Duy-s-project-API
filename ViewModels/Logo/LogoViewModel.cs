namespace DuyProject.API.ViewModels.Logo
{
    public class LogoViewModel
    {
        public LogoViewModel(string id, string imagePath, string pharmacyId, bool isActive)
        {
            Id = id;
            ImagePath = imagePath;
            PharmacyId = pharmacyId;
            IsActive = isActive;
        }

        public string Id { get; set; }
        public string ImagePath { get; set; }
        public string PharmacyId { get; set; }
        public bool IsActive { get; set; }
    }
}
