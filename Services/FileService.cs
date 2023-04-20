using DuyProject.API.Configurations;
using DuyProject.API.Helpers;
using DuyProject.API.Models;
using DuyProject.API.Repositories;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using DuyProject.API.ViewModels.File;
using MongoDB.Driver;

namespace DuyProject.API.Services
{
    public class FileService : IFileService
    {
        private readonly string _filesPath;
        private readonly IMongoCollection<FileDocument> _fileCollection;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IMongoClient mongoClient, IWebHostEnvironment webHostEnvironment)
        {
            IMongoDatabase? database = mongoClient.GetDatabase(AppSettings.DbName);
            _fileCollection = database.GetCollection<FileDocument>("Files");
            _webHostEnvironment = webHostEnvironment;
            _filesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Files");
        }

        public async Task<ServiceResult<FileViewModel>> SaveFileAsync(FileCreateCommand file)
        {
            string fileExtension = FileExtensionHelper.ReturnFileExtension(file);
            var fileName = Path.GetFileNameWithoutExtension(file.RecordId) + fileExtension;
            string targetPath = Path.Combine(_filesPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            var base64Data = file.FileContent.Split(',')[1];
            var fileContent = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(targetPath, fileContent);

            await _fileCollection.InsertOneAsync(new FileDocument
            {
                FilePath = targetPath,
                RecordId = file.RecordId,
            });

            var data = new FileViewModel
            {
                FileContent = base64Data,
                RecordId = file.RecordId,
                FileExtension = fileExtension,
                FileUrl = targetPath
            };

            return new ServiceResult<FileViewModel>(data);
        }


        public async Task<ServiceResult<FileViewModel>> ReadFileAsync(string recordId)
        {
            var record =  _fileCollection.AsQueryable().FirstOrDefault(x=>x.RecordId == recordId);
            if(record is null)
            {
                return new ServiceResult<FileViewModel>("No record founded");
            }
            byte[] bytes = File.ReadAllBytes(record.FilePath);
            string extension = Path.GetExtension(record.FilePath);
            string base64String = Convert.ToBase64String(bytes);
            
            var data = new FileViewModel
            {
                Id = record.Id,
                FileUrl = record.FilePath,
                RecordId = recordId,
                FileContent = base64String,
                FileExtension = extension,
            };

            return new ServiceResult<FileViewModel>(data);
        }
    }
}