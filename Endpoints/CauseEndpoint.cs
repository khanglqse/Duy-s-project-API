using AutoMapper;
using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Cause;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class CauseEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/causes", async (CauseService posService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                ServiceResult<PaginationResponse<CauseViewModel>> result = await posService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Causes").WithGroupName("Cause");

        app.MapGet("api/cause", async (CauseService posService, string id) =>
            {
                ServiceResult<CauseViewModel> result = await posService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Cause").WithGroupName("Cause");

        app.MapPost("api/cause", async (CauseService posService, IMapper mapper, CauseCreateCommand command) =>
            {
                ServiceResult<CauseViewModel> result = await posService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Cause").WithGroupName("Cause");

        app.MapPut("api/cause/{id}", async (CauseService posService, IMapper mapper, [FromRoute] string id,
                [FromBody] CauseUpdateCommand command) =>
            {
                ServiceResult<CauseViewModel> result = await posService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Cause").WithGroupName("Cause");

        app.MapDelete("api/cause/{id}", async (CauseService posService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await posService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Cause").WithGroupName("Cause");

        app.MapPut("api/cause/{id}/toggle",
                async (CauseService posService, string id) => { return Results.Ok(await posService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_CauseToggle").WithGroupName("Cause");

    }
}