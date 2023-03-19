using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels.Logo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class LogoEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/logos", async (LogoService posService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                var result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("GET_Logos");

        app.MapGet("api/logo", async (LogoService posService, string id) =>
            {
                var result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("GET_Logo");

        app.MapPost("api/logo", async (LogoService posService, IMapper mapper, LogoCreateCommand command) =>
            {
                var result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("POST_Logo");

        app.MapPut("api/logo/{id}", async (LogoService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] LogoUpdateCommand command) =>
            {
                var result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_Logo");

        app.MapDelete("api/logo/{id}", async (LogoService posService, [FromRoute] string id) =>
            {
                var result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme })
            .WithName("DELETE_Logo");

        app.MapPut("api/logo/{id}/toggle",
                async (LogoService posService, string id) => { return Results.Ok(await posService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_LogoToggle");

    }
}