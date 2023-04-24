namespace DuyProject.API.ViewModels.Drug
{
    public class DrugUpdateCommand
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Effect { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public string Quatity { get; set; }
    }
}