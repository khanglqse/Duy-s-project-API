using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
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
                ServiceResult<PaginationResponse<PharmacyViewModel>> result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Pharmacys").WithGroupName("Pharmacy");

        app.MapGet("api/pharmacy", async (PharmacyService posService, string id) =>
            {
                ServiceResult<PharmacyViewModel> result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Pharmacy").WithGroupName("Pharmacy");

        app.MapPost("api/pharmacy", async (PharmacyService posService, IMapper mapper, PharmacyCreateCommand command) =>
            {
                ServiceResult<PharmacyViewModel> result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Pharmacy").WithGroupName("Pharmacy");

        app.MapPut("api/pharmacy/{id}", async (PharmacyService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] PharmacyUpdateCommand command) =>
            {
                ServiceResult<PharmacyViewModel> result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Pharmacy").WithGroupName("Pharmacy");

        app.MapDelete("api/pharmacy/{id}", async (PharmacyService posService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Pharmacy").WithGroupName("Pharmacy");

        app.MapPut("api/pharmacy/{id}/toggle",
                async (PharmacyService posService, string id) => Results.Ok(await posService.ToggleActive(id)))
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_PharmacyToggle").WithGroupName("Pharmacy");

    }
}