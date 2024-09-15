using Microsoft.AspNetCore.Authentication;

namespace HelloJkwCore.Authentication;

public static class ExternalAuthenticationHelper
{
    public static AuthenticationBuilder AddGoogleAuthentication(this AuthenticationBuilder builder, OAuthConfig? googleAuthConfig)
    {
        if (googleAuthConfig != null)
        {
            builder.AddGoogle(options =>
            {
                options.ClientId = googleAuthConfig.ClientId ?? string.Empty;
                options.ClientSecret = googleAuthConfig.ClientSecret ?? string.Empty;
                options.CallbackPath = googleAuthConfig.Callback;
                options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                options.ClaimActions.MapJsonKey("urn:google:image", "picture");
            });
        }
        return builder;
    }
    public static AuthenticationBuilder AddKakaoAuthentication(this AuthenticationBuilder builder, OAuthConfig? kakaoAuthConfig)
    {
        if (kakaoAuthConfig != null)
        {
            builder.AddKakaoTalk(options =>
            {
                options.ClientId = kakaoAuthConfig.ClientId ?? string.Empty;
                options.ClientSecret = kakaoAuthConfig.ClientSecret ?? string.Empty;
                options.CallbackPath = kakaoAuthConfig.Callback;
            });
        }
        return builder;
    }
}
