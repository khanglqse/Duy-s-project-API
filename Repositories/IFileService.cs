using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.File;

namespace DuyProject.API.Repositories
{
    public interface IFileService
    {
        Task<ServiceResult<FileViewModel>> SaveFileAsync(FileCreateCommand file);
        Task<ServiceResult<FileViewModel>> ReadFileAsync(string recordId);
    }
}
