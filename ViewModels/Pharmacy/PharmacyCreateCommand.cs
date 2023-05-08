namespace DuyProject.API.ViewModels.Pharmacy
{
    public class PharmacyCreateCommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public string Phone { get; set; }
        public List<string> DrugIds { get; set; }
        public List<string> DoctorIds { get; set; }
        public double[] Coordinates { get; set; }
        public string Type { get; set; } = "Point";
        public string LogoId { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}