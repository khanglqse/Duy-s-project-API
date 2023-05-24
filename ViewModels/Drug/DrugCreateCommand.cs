namespace DuyProject.API.ViewModels.Drug
{
    public class DrugCreateCommand
    {
        public string Name { get; set; }
        public string Effect { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public string ReferenceImage { get; set; }
        public string Quantity { get; set; }
    }
}