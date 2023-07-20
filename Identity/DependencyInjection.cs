using Application.Contracts;
using Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddTransient<IAuthService, AuthService>();         
            return services;
        }
    }
}
