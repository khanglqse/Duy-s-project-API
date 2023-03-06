using DuyProject.API.Configurations;
using DuyProject.API.Models;
using DuyProject.API.ViewModels.User;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DuyProject.API.Services;

public class TokenService
{
    private readonly ApplicationSettings _settings;
    public TokenService(ApplicationSettings settings)
    {
        _settings = settings;
    }
    public TokenViewModel GetToken(User user, bool isRefresh = false)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Name, user.Name),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };
        claims.AddRange(user.Roles.Split(',').Select(role => new Claim(ClaimTypes.Role, role.Trim())));

        var token = new JwtSecurityToken
        (
            issuer: _settings.JWT.Issuer,
            audience: _settings.JWT.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.JWT.ExpirationMinutes),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWT.SigningKey)), SecurityAlgorithms.HmacSha256)
        );
        return new TokenViewModel { Token = new JwtSecurityTokenHandler().WriteToken(token), Expires = token.ValidTo };
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JWT.SigningKey)),
            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        bool isValid = securityToken.ValidTo.AddMinutes(_settings.JWT.RefreshMinutes) > DateTime.UtcNow;
        if (isValid)
        {
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        return null;
    }
}