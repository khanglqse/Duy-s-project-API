namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyCreateCommand
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public List<string> DrugIds { get; set; }
        public List<string> DoctorIds { get; set; }
        public string LogoId { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}