using DuyProject.API.Configurations;
using DuyProject.API.Endpoints;
using DuyProject.API.Helpers;
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

        public async Task<string> SaveFileAsync(FileModel file)
        {
            string fileName = FileExtensionHelper.ReturnFileNameWithExtension(file);
            string targetPath = Path.Combine(_filesPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            var base64Data = file.Content.Split(',')[1];
            var fileContent = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(targetPath, fileContent);
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