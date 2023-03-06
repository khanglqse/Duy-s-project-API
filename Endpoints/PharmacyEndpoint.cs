using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels.Pharmacy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class PharmacyEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/pharmacys", async (PharmacyService posService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                var result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Pharmacys");

        app.MapGet("api/pharmacy", async (PharmacyService posService, string id) =>
            {
                var result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Pharmacy");

        app.MapPost("api/pharmacy", async (PharmacyService posService, IMapper mapper, PharmacyCreateCommand command) =>
            {
                var result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Pharmacy");

        app.MapPut("api/pharmacy/{id}", async (PharmacyService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] PharmacyUpdateCommand command) =>
            {
                var result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Pharmacy");

        app.MapDelete("api/pharmacy/{id}", async (PharmacyService posService, [FromRoute] string id) =>
            {
                var result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Pharmacy");

        app.MapPut("api/pharmacy/{id}/toggle",
                async (PharmacyService posService, string id) => { return Results.Ok(await posService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute()
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_PharmacyToggle");

    }
}