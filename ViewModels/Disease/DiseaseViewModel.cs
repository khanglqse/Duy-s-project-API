namespace DuyProject.API.ViewModels.Disease
{
    public class DiseaseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> CauseIds { get; set; }
        public List<string> DrugIds { get; set; }
        public List<Models.Cause> Causes { get; set; }
        public List<Models.Drug> Drugs { get; set; }
        public string BasicExperiment { get; set; }
        public string Approach { get; set; }
        public string Treatment { get; set; }
        public string Diet { get; set; }
        public string LivingActivity { get; set; }
        public string ReferenceImage { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
