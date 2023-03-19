namespace DuyProject.API.ViewModels.Logo
{
    public class LogoCreateCommand
    {
        public LogoCreateCommand(string imagePath, string pharmacyId)
        {
            ImagePath = imagePath;
            PharmacyId = pharmacyId;
        }

        public string ImagePath { get; set; }
        public string PharmacyId { get; set; }
    }
}