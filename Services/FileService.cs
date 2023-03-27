using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.Repositories;
using MongoDB.Driver;

namespace DuyProject.API.Services
{
    public class FileService : IFileService
    {
        private readonly string _filesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Files");
        private readonly IMongoCollection<FileDocument> _fileCollection;

        public FileService(IMongoClient mongoClient)
        {
            IMongoDatabase? database = mongoClient.GetDatabase(AppSettings.DbName);
            _fileCollection = database.GetCollection<FileDocument>("Files");
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            string targetPath = Path.Combine(_filesPath, file.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            await using var fileStream = new FileStream(targetPath, FileMode.Create);
            await file.CopyToAsync(fileStream);
            return targetPath;
        }

        public async Task SaveFilePathAsync(string filePath)
        {
            var fileDocument = new FileDocument { FilePath = filePath };
            await _fileCollection.InsertOneAsync(fileDocument);
        }

        public async Task<string> ReadFileAsync(string fileName)
        {
            string filePath = Path.Combine(_filesPath, fileName);
            return await File.ReadAllTextAsync(filePath);
        }
    }
}