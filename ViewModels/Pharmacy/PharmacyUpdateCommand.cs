namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyUpdateCommand : PharmacyCreateCommand
    {
        public string Id { get; set; }
        public List<string> FollowUser { get; set; }
    }
}