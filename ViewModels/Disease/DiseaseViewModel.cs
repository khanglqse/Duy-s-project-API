namespace DuyProject.API.ViewModels.Disease
{
    public class DiseaseViewModel : DiseaseUpdateCommand
    {
        public List<Models.Symptoms> Symptoms { get; set; }
        public List<Models.Drug> Drugs { get; set; }
        public bool IsActive { get; set; }
    }
}
