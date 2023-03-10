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
        app.MapGet("api/causes", async (CauseService causeService, string? filterValue, int? pageNumber, int? pageSize) =>
            {
                ServiceResult<PaginationResponse<CauseViewModel>> result = await causeService.List(pageNumber ?? 1, pageSize ?? AppSettings.DefaultPageSize,
                    filterValue);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Causes").WithGroupName("Cause");

        app.MapGet("api/cause", async (CauseService causeService, string id) =>
            {
                ServiceResult<CauseViewModel> result = await causeService.Get(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("GET_Cause").WithGroupName("Cause");

        app.MapPost("api/cause", async (CauseService causeService, IMapper mapper, CauseCreateCommand command) =>
            {
                ServiceResult<CauseViewModel> result = await causeService.Create(command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("POST_Cause").WithGroupName("Cause");

        app.MapPut("api/cause/{id}", async (CauseService causeService, IMapper mapper, [FromRoute] string id,
                [FromBody] CauseUpdateCommand command) =>
            {
                ServiceResult<CauseViewModel> result = await causeService.Update(id, command);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole }).AllowAnonymous()
            .WithName("PUT_Cause").WithGroupName("Cause");

        app.MapDelete("api/cause/{id}", async (CauseService causeService, [FromRoute] string id) =>
            {
                ServiceResult<object> result = await causeService.Remove(id);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme }).AllowAnonymous()
            .WithName("DELETE_Cause").WithGroupName("Cause");

        app.MapPut("api/cause/{id}/toggle",
                async (CauseService causeService, string id) => { return Results.Ok(await causeService.ToggleActive(id)); })
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_CauseToggle").WithGroupName("Cause");

    }
}