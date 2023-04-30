namespace AuthApi.Common.ConfigClasses;

public sealed class JwtConfig
{
    public required string Issuer { get; set; }
    
    public required string Audience { get; set; }
    
    public required string Key { get; set; }
    
    public required int AccessMinutesLifeTime { get; set; }
    
    public required int RefreshDaysLifeTime { get; set; }
}