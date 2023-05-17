using DuyProject.API.ViewModels;
public class DiagnoseRequestModel : PaginationRequest
{
    public string[]? Symptoms { get; set; }
}