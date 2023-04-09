using DuyProject.API.Repositories;
using DuyProject.API.ViewModels.File;

namespace DuyProject.API.Endpoints
{
    public static class FileEndpoint
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/upload", async (IFileService fileService, FileCreateCommand file) =>
            {
                var result = await fileService.SaveFileAsync(file);
                return Results.Ok(result);
            });

            app.MapGet("/file", async (string recordId, IFileService fileService) =>
            {
                var result = await fileService.ReadFileAsync(recordId);
                return Results.Ok(result);
            });
        }
    }
}
