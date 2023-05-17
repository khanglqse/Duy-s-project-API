using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class DiseaseEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/diseases", async (DiseaseService diseaseService, string? filterValue, int? pageNumber, int? pageSize) =>
        {
            ServiceResult<PaginationResponse<DiseaseViewModel>> result = await diseaseService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                filterValue);
            return Results.Ok(result);
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
        .AllowAnonymous()
        .WithName("GET_Diseases").WithGroupName("Diseases");

        app.MapGet("api/disease", async (DiseaseService diseaseService, string id) =>
        {
            ServiceResult<DiseaseViewModel> result = await diseaseService.Get(id);
            return Results.Ok(result);
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
        .AllowAnonymous()
        .WithName("GET_Disease").WithGroupName("Diseases");

        app.MapGet("api/diagnose", async (DiseaseService diseaseService, string diseaseId, string? userId, string? address) =>
        {
            ServiceResult<DiagnoseModel> result = await diseaseService.GetDiagnoseInfo(diseaseId, userId, address);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .WithName("GET_Diagnose").WithGroupName("Diseases");

        app.MapPost("api/disease", async (DiseaseService diseaseService, IMapper mapper, DiseaseCreateCommand command) =>
        {
            ServiceResult<DiseaseViewModel> result = await diseaseService.Create(command);
            return Results.Ok(result);
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
        .AllowAnonymous()
        .WithName("POST_Disease").WithGroupName("Diseases");

        app.MapPut("api/disease/{id}", async (DiseaseService diseaseService, IMapper mapper, [FromRoute] string id, [FromBody] DiseaseUpdateCommand command) =>
        {
            ServiceResult<DiseaseViewModel> result = await diseaseService.Update(id, command);
            return Results.Ok(result);
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
        .AllowAnonymous()
        .WithName("PUT_Disease").WithGroupName("Diseases");

        app.MapDelete("api/disease/{id}", async (DiseaseService diseaseService, [FromRoute] string id) =>
        {
            ServiceResult<object> result = await diseaseService.Remove(id);
            return Results.Ok(result);
        })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme })
        .AllowAnonymous()
        .WithName("DELETE_Disease").WithGroupName("Diseases");

        app.MapPut("api/disease/{id}/toggle", async (DiseaseService diseaseService, string id) => { return Results.Ok(await diseaseService.ToggleActive(id)); })
        .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
        .WithName("PUT_DiseaseToggle").WithGroupName("Diseases");



        app.MapPost("api/diagnosis", async (DiseaseService diseaseService, DiagnoseRequestModel request) =>
        {
            var result = await diseaseService.Diagnosis(request);
            return Results.Ok(result);
        })
        .Produces<PaginationResponse<DiseaseModel>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .AllowAnonymous()
        .WithName("GET_Diagnosis").WithGroupName("Diagnosis");
    }
}