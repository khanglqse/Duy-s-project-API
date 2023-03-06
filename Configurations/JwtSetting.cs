namespace DuyProject.API.Configurations;

public class JwtSetting
{
    public string SigningKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int ExpirationMinutes { get; set; }
    public int RefreshMinutes { get; set; }
}