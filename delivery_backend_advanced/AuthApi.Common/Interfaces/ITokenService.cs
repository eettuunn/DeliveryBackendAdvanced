using System.Security.Claims;
using AuthApi.Common.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Common.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(TokenUserDto tokenUserDto, List<IdentityRole> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetExpiredTokenInfo(string? accessToken);
}