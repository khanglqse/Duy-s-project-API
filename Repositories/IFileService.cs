namespace DuyProject.API.Repositories
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task SaveFilePathAsync(string filePath);
        Task<string> ReadFileAsync(string fileName);
    }
}
