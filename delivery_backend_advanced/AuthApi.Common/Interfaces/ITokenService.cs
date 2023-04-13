using AuthApi.Common.Dtos;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Common.Interfaces;

public interface ITokenService
{
    string CreateToken(TokenUserDto tokenUserDto, List<IdentityRole> roles);
    string GenerateRefreshToken();
}