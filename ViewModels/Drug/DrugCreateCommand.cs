namespace DuyProject.API.ViewModels.Drug
{
    public class DrugCreateCommand
    {
        public string Name { get; set; }
        public string Effect { get; set; }
        public string Description { get; set; }
        public double Price { get; set; } = 0;
        public string Type { get; set; }
        public int Quantity { get; set; } = 0;
    }
}