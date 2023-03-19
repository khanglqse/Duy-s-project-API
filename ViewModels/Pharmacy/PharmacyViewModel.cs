namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<string> DrugIds { get; set; } = new List<string>();
        public string Phone { get; set; } = string.Empty;
        public string LogoId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
