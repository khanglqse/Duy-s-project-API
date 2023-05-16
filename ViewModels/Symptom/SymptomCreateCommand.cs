namespace DuyProject.API.ViewModels.Symptoms
{
    public class SymptomCreateCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReferenceImage { get; set; }
        public string Type { get; set; }
    }
}