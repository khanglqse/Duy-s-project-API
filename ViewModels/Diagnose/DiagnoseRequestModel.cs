using DuyProject.API.Configurations;
using DuyProject.API.ViewModels;
public class DiagnoseRequestModel : PaginationRequest
{
    private static char[] delimiterChars = { ' ', ',' };
    public string[]? Symptoms { get; set; }

    public static ValueTask<DiagnoseRequestModel?> BindAsync(HttpContext context)
    {
        int.TryParse(context.Request.Query[nameof(PageNumber)], out var page);
        int.TryParse(context.Request.Query[nameof(PageSize)], out var pageSize);
        bool.TryParse(context.Request.Query[nameof(OrderAsc)], out var orderAsc);
        var symptomsTemp = context.Request.Query[nameof(Symptoms)];
        var symptoms = new List<string>();
        foreach (var symptom in symptomsTemp.Where(symptoms => !string.IsNullOrWhiteSpace(symptoms)))
        {
            symptoms.AddRange(symptom.Trim().Split(delimiterChars));
        }

        var result = new DiagnoseRequestModel
        {
            Symptoms = symptoms.ToArray(),
            PageNumber = page < 1 ? AppSettings.DefaultPage : page,
            PageSize = pageSize < 1 ? AppSettings.DefaultPageSize : pageSize,
            FilterValue = context.Request.Query[nameof(FilterValue)],
            OrderBy = context.Request.Query[nameof(OrderBy)],
            OrderAsc = orderAsc,
        };

        return ValueTask.FromResult<DiagnoseRequestModel?>(result);
    }
}