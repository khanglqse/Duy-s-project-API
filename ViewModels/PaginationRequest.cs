using DuyProject.API.Configurations;

namespace DuyProject.API.ViewModels;

public class PaginationRequest
{
    private int _pageNumber;
    private int _pageSize;

    public int PageNumber 
    { 
        get => this._pageNumber;
        set 
        {
            _pageNumber = value < 1 ?  AppSettings.DefaultPage : value;
        } 
    }
    public int PageSize 
    { 
        get => this._pageSize;
        set 
        {
            _pageSize = value < 1? AppSettings.DefaultPageSize : value;
        }
    }
    public string? FilterValue { get; set;}
    public string? OrderBy { get; set; } = string.Empty;
    public bool OrderAsc { get; set; }
}