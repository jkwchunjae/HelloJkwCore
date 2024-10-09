namespace HelloJkwCore.Authentication;

public class OAuthConfig
{
    public AuthProvider Provider { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Callback { get; set; }
    public string? RefreshToken { get; set; }
}
