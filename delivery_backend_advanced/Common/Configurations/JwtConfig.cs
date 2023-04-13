using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Common.Configurations;

public class JwtConfig
{
    public const string Issuer = "kvolikdub_backend";              
    public const string Audience = "kvolikdub_frontend";                  
    private const string Key = "TheVeryStrongKeyOrPasswordQwerty123";
    public const int Lifetime = 5;
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}