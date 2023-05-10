using System;
using Azure.Storage.Blobs.Models;

namespace DuyProject.API.Interfaces
{
	public interface IAzureBlobStorageService
	{
        Task<Azure.Response<BlobContentInfo>> UploadFile(IFormFile file, string folder);
        Task<List<Azure.Response<BlobContentInfo>>> UploadFiles(IFormFileCollection files, string folder);
        Task<List<BlobItem>> GetAttachmentFiles();
        Task<Uri> GetUrl(string fileName);
        Task<bool> DeleteBlob(string blobName);
    }
}
