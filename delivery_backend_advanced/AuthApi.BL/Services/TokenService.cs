using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthApi.Common.Dtos;
using AuthApi.Common.Interfaces;
using Common.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.BL.Services;

public class TokenService : ITokenService
{
    public string CreateToken(TokenUserDto tokenUserDto, List<IdentityRole> roles)
    {
        var newToken = CreateJwtToken(CreateClaims(tokenUserDto, roles));
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return tokenHandler.WriteToken(newToken);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    
    
    public static List<Claim> CreateClaims(TokenUserDto user, List<IdentityRole> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.id.ToString()),
            new(ClaimTypes.Name, user.username),
            new(ClaimTypes.Email, user.email)
            
        };
        
        foreach (var r in roles)
        {
            claims.Add(new(ClaimTypes.Role, r.Name));
        }
        return claims;
    }

    public static JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims)
    {
        return new JwtSecurityToken(
            JwtConfig.Issuer,
            JwtConfig.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(JwtConfig.JwtLifetime),
            signingCredentials: new SigningCredentials(JwtConfig.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256)
        );
    }
    
    public ClaimsPrincipal? GetExpiredTokenInfo(string? accessToken)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = JwtConfig.GetSymmetricSecurityKey(),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}