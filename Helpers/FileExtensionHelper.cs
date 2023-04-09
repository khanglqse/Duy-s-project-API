using DuyProject.API.Models;
using DuyProject.API.ViewModels.File;
using MongoDB.Driver;

namespace DuyProject.API.Helpers
{
    public static class FileExtensionHelper
    {
        public static string ReturnFileExtension(FileCreateCommand file)
        {
            var mimeTypes = new[]
            {
                 new { Extension = ".jpg", MimeType = "image/jpeg" },
                 new { Extension = ".png", MimeType = "image/png" },
                 new { Extension = ".gif", MimeType = "image/gif" },
                 new { Extension = ".jpg", MimeType = "@file/jpeg" },
                 new { Extension = ".png", MimeType = "@file/png" },
                 new { Extension = ".gif", MimeType = "@file/gif" },
                 new { Extension = ".txt", MimeType = "text/plain" },
                 new { Extension = ".pdf", MimeType = "application/pdf" },
                 new { Extension = ".doc", MimeType = "application/msword" },
                 new { Extension = ".docx", MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                 new { Extension = ".xls", MimeType = "application/vnd.ms-excel" },
                 new { Extension = ".xlsx", MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                 new { Extension = ".ppt", MimeType = "application/vnd.ms-powerpoint" },
                 new { Extension = ".pptx", MimeType = "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                 new { Extension = ".zip", MimeType = "application/zip" }
            };
            var mimeType = file.FileContent.Split(';')[0].Split(':')[1];
            var fileExtension = mimeTypes.FirstOrDefault(mt => mt.MimeType == mimeType)?.Extension;
            return fileExtension;
        }
    }
}
