using DuyProject.API.Configurations;
using DuyProject.API.Services;
using DuyProject.API.ViewModels;
using DuyProject.API.ViewModels.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DuyProject.API.Endpoints;

public static class UserEndpoint
{
    public static void Map(WebApplication app)
    {
        app.MapPost("api/authenticate/login", async (UserService userService, LoginCommand request) =>
            {
                ServiceResult<LoginViewModel> result = await userService.Login(request);
                if (result.Success)
                {
                    return Results.Ok(result);
                }

                return Results.NoContent();
            })
            .AllowAnonymous()
            .WithName("POST_Login");

        app.MapPost("api/authenticate/googleLogin", async (UserService userService, GoogleLoginCommand googleLoginCommand) =>
            {
                ServiceResult<LoginViewModel> result = await userService.LoginWithGoogle(googleLoginCommand);
                if (result.Success)
                {
                    return Results.Ok(result);
                }

                return Results.NoContent();
            })
            .AllowAnonymous()
            .WithName("POST_GoogleLogin");

        app.MapPost("api/authenticate/facebook-login", async (UserService userService, FacebookLoginCommand facebookLoginCommand) =>
            {
                ServiceResult<LoginViewModel> result = await userService.LoginWithFacebook(facebookLoginCommand);
                if (result.Success)
                {
                    return Results.Ok(result);
                }

                return Results.NoContent();
            })
            .AllowAnonymous()
            .WithName("POST_FacebookLogin");

        app.MapPut("api/authenticate/{id}", async (UserService userService, [FromRoute] string id,
                [FromBody] UserUpdateCommand userViewModel) =>
            {
                ServiceResult<UserViewModel> result = await userService.Update(id, userViewModel);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = AppSettings.AdminRole })
            .WithName("PUT_User");

        app.MapPost("api/authenticate/refresh-token", async (UserService userService, RefreshTokenCommand command) =>
            {
                ServiceResult<LoginViewModel> result = await userService.RefreshToken(command);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("POST_RefreshToken");

        app.MapPost("api/authenticate/register", async (UserService userService, UserCreateCommand command) =>
            {
                ServiceResult<UserViewModel> result = await userService.Create(command);
                return Results.Ok(result);
            })
            .AllowAnonymous()
            .WithName("POST_CreateUser");

    }
}