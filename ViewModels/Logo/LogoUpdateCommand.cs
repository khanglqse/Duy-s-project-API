namespace DuyProject.API.ViewModels.Logo
{
    public class LogoUpdateCommand
    {
        public LogoUpdateCommand(string id, string imagePath, string pharmacyId)
        {
            Id = id;
            ImagePath = imagePath;
            PharmacyId = pharmacyId;
        }

        public string Id { get; set; }
        public string ImagePath { get; set; }
        public string PharmacyId { get; set; }
    }
}