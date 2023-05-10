using System.ComponentModel.DataAnnotations;
using DuyProject.API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Controllers;

[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "BlobStorage")]
[ApiController]
public class AzureController : ControllerBase
{
    private readonly IAzureBlobStorageService _azureBlobService;

    public AzureController(IAzureBlobStorageService azureBlobService)
    {
        _azureBlobService = azureBlobService;
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("files")]
    public async Task<IResult> GetAttachmentFiles()
    {
        try
        {
            var result = await _azureBlobService.GetAttachmentFiles();
            return Results.Ok(result);
        }
        catch (FileNotFoundException fileEx)
        {
            return Results.NotFound(fileEx.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("file")]
    public async Task<IResult> GetAttachmentFile([Required]string fileName)
    {
        try
        {
            var result = await _azureBlobService.GetUrl(fileName);
            return Results.Ok(result);
        }
        catch(Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("file")]
    public async Task<IResult> UploadFile([Required] IFormFile file, [Required] string folder)
    {
        var result = await _azureBlobService.UploadFile(file, folder);
        return Results.Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("files")]
    public async Task<IResult> UploadFiles([Required]IFormFileCollection file, [Required] string folder)
    {
        if (!file.Any())
        {
            return Results.BadRequest("There are no files");
        }

        var result = await _azureBlobService.UploadFiles(file, folder);
        return Results.Ok(result);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("blob")]
    public async Task<IResult> DeleteBlob([Required] string blobName)
    {
        return Results.Ok(await _azureBlobService.DeleteBlob(blobName));
    }
}