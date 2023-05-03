using DuyProject.API.Configurations;
using DuyProject.API.ViewModels;
public class DiagnoseRequestModel : PaginationRequest
{
    public string[]? Causes { get; set; }

    public static ValueTask<DiagnoseRequestModel?> BindAsync(HttpContext context)
    {
        int.TryParse(context.Request.Query[nameof(PageNumber)], out var page);
        int.TryParse(context.Request.Query[nameof(PageSize)], out var pageSize);
        bool.TryParse(context.Request.Query[nameof(OrderAsc)], out var orderAsc);
        var causesTemp = context.Request.Query[nameof(Causes)];
        var causes = new List<string>();
        foreach (var cause in causesTemp.Where(cause => !string.IsNullOrEmpty(cause)))
        {
            causes.Add(cause.Trim());
        }

        var result = new DiagnoseRequestModel
        {
            Causes = causes.ToArray(),
            PageNumber = page < 1 ? AppSettings.DefaultPage : page,
            PageSize = pageSize < 1 ? AppSettings.DefaultPageSize : pageSize,
            FilterValue = context.Request.Query[nameof(FilterValue)],
            OrderBy = context.Request.Query[nameof(OrderBy)],
            OrderAsc = orderAsc,
        };

        return ValueTask.FromResult<DiagnoseRequestModel?>(result);
    }
}