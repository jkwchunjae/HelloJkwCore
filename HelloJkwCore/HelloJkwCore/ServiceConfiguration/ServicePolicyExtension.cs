using Microsoft.Extensions.DependencyInjection;

namespace HelloJkwCore;

public static class ServicePolicyExtension
{
    public static IServiceCollection AddHelloJkwPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Diary",
                policy => policy.RequireAuthenticatedUser());
        });

        return services;
    }
}