using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Symptoms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class SymptomsEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/Symptoms", async (SymptomsService symptomsService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                ServiceResult<PaginationResponse<SymptomsViewModel>> result = await symptomsService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Symptoms").WithGroupName("Symptoms");

        app.MapGet("api/symptom", async (SymptomsService symptomsService, string id) =>
            {
                ServiceResult<SymptomsViewModel> result = await symptomsService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Symptom").WithGroupName("Symptoms");

        app.MapPost("api/symptoms", async (SymptomsService symptomsService, IMapper mapper, SymptomsCreateCommand command) =>
            {
                ServiceResult<SymptomsViewModel> result = await symptomsService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Symptoms").WithGroupName("Symptoms");

        app.MapPut("api/symptoms/{id}", async (SymptomsService symptomsService, IMapper mapper, [FromRoute] string id,
                [FromBody] SymptomsUpdateCommand command) =>
            {
                ServiceResult<SymptomsViewModel> result = await symptomsService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Symptoms").WithGroupName("Symptoms");

        app.MapDelete("api/symptoms/{id}", async (SymptomsService symptomsService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await symptomsService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Symptoms").WithGroupName("Symptoms");

        app.MapPut("api/symptoms/{id}/toggle",
                async (SymptomsService symptomsService, string id) => { return Results.Ok(await symptomsService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_SymptomsToggle").WithGroupName("Symptoms");

    }
}