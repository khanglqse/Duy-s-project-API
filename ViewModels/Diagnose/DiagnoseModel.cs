using DuyProject.API.ViewModels.Pharmacy;

namespace DuyProject.API.ViewModels.Disease
{
    public class DiagnoseModel
    {
        public Models.Disease Disease { get; set; }
        public List<PharmacyViewModel> Pharmacies { get; set; }
    }
}
