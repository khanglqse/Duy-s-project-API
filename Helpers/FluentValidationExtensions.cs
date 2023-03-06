using DuyProject.API.ViewModels;
using FluentValidation.Results;

namespace DuyProject.API.Helpers;

public static class FluentValidationExtensions
{
    public static ServiceResult<string> ToListStringError(this ValidationResult validationResult)
    {
        IEnumerable<string> errorList = validationResult.Errors.Select(x => x.ErrorMessage);
        string stringError = string.Join("\\n", errorList);
        return new ServiceResult<string>(stringError);
    }
}