using DuyProject.API.ViewModels.Pharmacy;

namespace DuyProject.API.ViewModels.Disease
{
    public class DiagnoseModel
    {
        public DiagnoseModel(DiseaseViewModel disease) => Disease = disease;

        public DiagnoseModel(DiseaseViewModel disease, List<PharmacyViewModel> pharmacies)
        {
            new DiagnoseModel(disease);
            Pharmacies = pharmacies;
        }

        public DiseaseViewModel Disease { get; set; }
        public List<PharmacyViewModel> Pharmacies { get; set; }
    }
}
