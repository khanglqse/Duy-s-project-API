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
        app.MapGet("api/drugs", async (DrugService drugService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                ServiceResult<PaginationResponse<DrugViewModel>> result = await drugService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("GET_Drugs").WithGroupName("Drug");

        app.MapGet("api/drug", async (DrugService drugService, string id) =>
            {
                ServiceResult<DrugViewModel> result = await drugService.Get(id);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("GET_Drug").WithGroupName("Drug");

        app.MapPost("api/drug", async (DrugService drugService, IMapper mapper, DrugCreateCommand command) =>
            {
                ServiceResult<DrugViewModel> result = await drugService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("POST_Drug").WithGroupName("Drug");

        app.MapPut("api/drug/{id}", async (DrugService drugService, IMapper mapper, [FromRoute] string id,
                [FromBody] DrugUpdateCommand command) =>
            {
                ServiceResult<DrugViewModel> result = await drugService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_Drug").WithGroupName("Drug");

        app.MapDelete("api/drug/{id}", async (DrugService drugService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await drugService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("DELETE_Drug").WithGroupName("Drug");

        app.MapPut("api/drug/{id}/toggle",
                async (DrugService drugService, string id) => Results.Ok(await drugService.ToggleActive(id)))
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_DrugToggle").WithGroupName("Drug");

    }
}