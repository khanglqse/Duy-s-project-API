using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
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
                ServiceResult<PaginationResponse<DrugViewModel>> result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Drugs").WithGroupName("Drug");

        app.MapGet("api/drug", async (DrugService posService, string id) =>
            {
                ServiceResult<DrugViewModel> result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Drug").WithGroupName("Drug");

        app.MapPost("api/drug", async (DrugService posService, IMapper mapper, DrugCreateCommand command) =>
            {
                ServiceResult<DrugViewModel> result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Drug").WithGroupName("Drug");

        app.MapPut("api/drug/{id}", async (DrugService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] DrugUpdateCommand command) =>
            {
                ServiceResult<DrugViewModel> result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Drug").WithGroupName("Drug");

        app.MapDelete("api/drug/{id}", async (DrugService posService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Drug").WithGroupName("Drug");

        app.MapPut("api/drug/{id}/toggle",
                async (DrugService posService, string id) => Results.Ok(await posService.ToggleActive(id)))
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_DrugToggle").WithGroupName("Drug");

    }
}