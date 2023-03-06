using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels.Disease;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class DiseaseEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/diseases", async (DiseaseService posService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                var result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Diseases");

        app.MapGet("api/disease", async (DiseaseService posService, string id) =>
            {
                var result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Disease");

        app.MapPost("api/disease", async (DiseaseService posService, IMapper mapper, DiseaseCreateCommand command) =>
            {
                var result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Disease");

        app.MapPut("api/disease/{id}", async (DiseaseService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] DiseaseUpdateCommand command) =>
            {
                var result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Disease");

        app.MapDelete("api/disease/{id}", async (DiseaseService posService, [FromRoute] string id) =>
            {
                var result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Disease");

        app.MapPut("api/disease/{id}/toggle",
                async (DiseaseService posService, string id) => { return Results.Ok(await posService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_DiseaseToggle");

    }
}