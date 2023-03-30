using DuyProject.API.Repositories;

namespace DuyProject.API.Endpoints
{
    public static class FileEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/upload", async (FileModel file, IFileService fileService) =>
            {
                var filePath = await fileService.SaveFileAsync(file);
                await fileService.SaveFilePathAsync(filePath);
                return Results.Text(filePath);
            });

            app.MapGet("/file", async (string fileUrl, IFileService fileService) =>
            {
                var fileName = Path.GetFileName(fileUrl);
                var fileContent = await fileService.ReadFileAsync(fileName);
                return Results.Text(fileContent);
            });
        }
    }

    public class FileModel
    {
        public string FileName { get; set; }
        public string Content { get; set; }
    }
}
