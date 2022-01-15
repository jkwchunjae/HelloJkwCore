using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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