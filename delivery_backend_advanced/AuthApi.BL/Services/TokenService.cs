using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthApi.Common.ConfigClasses;
using AuthApi.Common.Dtos;
using AuthApi.Common.Enums;
using AuthApi.Common.Interfaces;
using AuthApi.DAL;
using AuthApi.DAL.Entities;
using Common.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthApi.BL.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AuthDbContext _context;

    public TokenService(IConfiguration configuration, AuthDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<string> CreateToken(TokenUserDto tokenUserDto, List<IdentityRole> roles)
    {
        var newToken = CreateJwtToken(await CreateClaims(tokenUserDto, roles));
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
    
    
    
    private async Task<List<Claim>> CreateClaims(TokenUserDto user, List<IdentityRole> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.id.ToString()),
            new(ClaimTypes.Name, user.username),
            new(ClaimTypes.Email, user.email),
            new("ban", user.isBanned.ToString())
        };
        
        foreach (var r in roles)
        {
            claims.Add(new(ClaimTypes.Role, r.Name));
        }

        if (roles.Any(r => r.Name == UserRole.Customer.ToString()))
        {
            var customer = await _context
                .Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.User.Id == user.id.ToString());
            claims.Add(new ("address", customer.Address));
        }
        return claims;
    }

    private JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims)
    {
        var jwtConfig = _configuration.GetSection("JwtConfig").Get<JwtConfig>();
        return new JwtSecurityToken(
            jwtConfig.Issuer,
            jwtConfig.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtConfig.AccessMinutesLifeTime),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)),
                SecurityAlgorithms.HmacSha256)
        );
    }
    
    public ClaimsPrincipal? GetExpiredTokenInfo(string? accessToken)
    {
        var jwtConfig = _configuration.GetSection("JwtConfig").Get<JwtConfig>();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}