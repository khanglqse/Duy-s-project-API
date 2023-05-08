using System.Security.Claims;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.Disease;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace DuyProject.API.Endpoints;

public static class DiagnoseEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapGet("api/diagnosis", async (DiagnoseService diagnoseService, DiagnoseRequestModel request, ClaimsPrincipal user) =>
            {
                var nameIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await diagnoseService.Diagnosis(nameIdentifier, request);
                return Results.Ok(result);
            })
            .Produces<PaginationResponse<DiagnoseModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme })
            .WithName("GET_Diagnosis").WithGroupName("Diagnosis");
    }
}