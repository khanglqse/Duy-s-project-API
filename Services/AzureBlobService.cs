using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using DuyProject.API.Configurations;
using DuyProject.API.Interfaces;
using Microsoft.Extensions.Options;

namespace DuyProject.API.Services
{
    public class AzureBlobService: IAzureBlobStorageService
    {
        private static long limitSize = 50 * 1024 * 1024; // 50 Mb
        private BlobContainerClient _containerClient;
        private readonly BlobStorageSettings _settings;

        public AzureBlobService(IOptions<BlobStorageSettings> settings)
        {
            _settings = settings.Value;
            _containerClient = new BlobContainerClient(_settings.ConnectionString, _settings.ContainerName);
        }

        public async Task<Azure.Response<BlobContentInfo>> UploadFile(IFormFile file, string folder)
        {
            Azure.Response<BlobContentInfo> client;
            if (file.Length > limitSize)
            {
                throw new Exception("Please Upload a file upto 50 mb!");
            }

            string fileName = file.FileName;
            BlobClient blobClient = _containerClient.GetBlobClient($"{folder}/{fileName}");

            await using (Stream? data = file.OpenReadStream())
            {
                client = await blobClient.UploadAsync(data);
            }

            return client;
        }

        public async Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(IFormFileCollection files, string folder)
        {
            List<Azure.Response<BlobContentInfo>> result = new List<Azure.Response<BlobContentInfo>>();
            foreach (var file in files)
            {
                result.Add(await UploadFile(file, folder));
            }

            return result;
        }

        public async Task<List<BlobItem>> GetAttachmentFiles()
        {
            var items = new List<BlobItem>();
            var uploadedFiles = _containerClient.GetBlobsAsync();
            await _containerClient.CreateIfNotExistsAsync();

            if (uploadedFiles == null || uploadedFiles.GetAsyncEnumerator().Current == null)
            {
                throw new FileNotFoundException("Blob is empty");
            }

            await foreach (BlobItem file in uploadedFiles)
            {
                items.Add(file);
            }

            return items;
        }

        public async Task<Uri> GetUrl(string path)
        {
            var blobClient = _containerClient.GetBlobClient(path);

            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException("Blob is empty");
            }

            return blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddMinutes(15));
        }

        public async Task<bool> DeleteBlob(string blobName)
        {
            return await _containerClient.DeleteBlobIfExistsAsync(blobName);
        }
    }
}
