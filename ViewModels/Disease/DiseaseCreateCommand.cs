namespace DuyProject.API.ViewModels.Disease
{
    public class DiseaseCreateCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Cause { get; set; }
        public string BasicExperiment { get; set; }
        public string Approach { get; set; }
        public string Treatment { get; set; }
        public string Diet { get; set; }
        public string LivingActivity { get; set; }
        public string ReferenceImage { get; set; }
        public string Type { get; set; }
    }
}