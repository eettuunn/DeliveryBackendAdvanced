using Microsoft.AspNetCore.Authorization;

namespace delivery_backend_advanced.Policies;

public class BanPolicy : IAuthorizationRequirement
{
    public BanPolicy()
    {
        
    }
}