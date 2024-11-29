// dependency injection
using HospitalManagementSystem.Services;
using NuGet.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddTransient<TokenService>();
            services.AddTransient<UserService>();
            services.AddTransient<MessageService>();
            return services;
        }
    }
}