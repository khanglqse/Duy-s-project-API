using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels.Drug;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class DrugEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/drugs", async (DrugService posService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                var result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Drugs");

        app.MapGet("api/drug", async (DrugService posService, string id) =>
            {
                var result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Drug");

        app.MapPost("api/drug", async (DrugService posService, IMapper mapper, DrugCreateCommand command) =>
            {
                var result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Drug");

        app.MapPut("api/drug/{id}", async (DrugService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] DrugUpdateCommand command) =>
            {
                var result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Drug");

        app.MapDelete("api/drug/{id}", async (DrugService posService, [FromRoute] string id) =>
            {
                var result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Drug");

        app.MapPut("api/drug/{id}/toggle",
                async (DrugService posService, string id) => { return Results.Ok(await posService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_DrugToggle");

    }
}