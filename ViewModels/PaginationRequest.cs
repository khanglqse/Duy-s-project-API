using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels;

public class PaginationRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = AppSettings.DefaultPageSize;
    public string OrderBy { get; set; } = string.Empty;
    public bool OrderAsc { get; set; }
}