namespace DuyProject.API.ViewModels;

public class PaginationResponse<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalItems { get; set; }
    public IEnumerable<T> Items { get; set; }
    public long TotalPages => TotalItems / PageSize + 1;
}