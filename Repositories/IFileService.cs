using DuyProject.API.Endpoints;

namespace DuyProject.API.Repositories
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(FileModel file);
        Task SaveFilePathAsync(string filePath);
        Task<string> ReadFileAsync(string fileName);
    }
}
