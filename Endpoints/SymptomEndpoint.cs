using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Symptoms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class SymptomEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/Symptoms", async (SymptomService symptomsService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                ServiceResult<PaginationResponse<SymptomViewModel>> result = await symptomsService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Symptoms").WithGroupName("Symptoms");

        app.MapGet("api/symptom", async (SymptomService symptomsService, string id) =>
            {
                ServiceResult<SymptomViewModel> result = await symptomsService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Symptom").WithGroupName("Symptoms");

        app.MapPost("api/symptom", async (SymptomService symptomsService, IMapper mapper, SymptomCreateCommand command) =>
            {
                ServiceResult<SymptomViewModel> result = await symptomsService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Symptom").WithGroupName("Symptoms");

        app.MapPut("api/symptom/{id}", async (SymptomService symptomsService, IMapper mapper, [FromRoute] string id,
                [FromBody] SymptomUpdateCommand command) =>
            {
                ServiceResult<SymptomViewModel> result = await symptomsService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Symptom").WithGroupName("Symptoms");

        app.MapDelete("api/symptom/{id}", async (SymptomService symptomsService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await symptomsService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Symptom").WithGroupName("Symptoms");

        app.MapPut("api/symptom/{id}/toggle",
                async (SymptomService symptomsService, string id) => { return Results.Ok(await symptomsService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_SymptomToggle").WithGroupName("Symptoms");

    }
}