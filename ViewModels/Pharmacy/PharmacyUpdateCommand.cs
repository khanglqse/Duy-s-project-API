namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyUpdateCommand : PharmacyCreateCommand
    {
        public List<string> FollowUser { get; set; }
    }
}