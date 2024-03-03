using Microsoft.AspNetCore.Authentication;

namespace HelloJkwCore.Authentication;

public static class ExternalAuthenticationHelper
{
    public static AuthenticationBuilder AddGoogleAuthentication(this AuthenticationBuilder builder, AuthUtil authUtil)
    {
        OAuthConfig? config = authUtil.GetAuthOption(AuthProvider.Google);

        if (config != null)
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = config?.ClientId ?? string.Empty;
                options.ClientSecret = config?.ClientSecret ?? string.Empty;
                options.CallbackPath = config?.Callback;
                options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                options.ClaimActions.MapJsonKey("urn:google:image", "picture");
            });
        }
        return builder;
    }
    public static AuthenticationBuilder AddKakaoAuthentication(this AuthenticationBuilder builder, AuthUtil authUtil)
    {
        OAuthConfig? config = authUtil.GetAuthOption(AuthProvider.KakaoTalk);

        if (config != null)
        {
            builder.AddKakaoTalk(options =>
            {
                options.ClientId = config?.ClientId ?? string.Empty;
                options.ClientSecret = config?.ClientSecret ?? string.Empty;
                options.CallbackPath = config?.Callback;
            });
        }
        return builder;
    }
}
