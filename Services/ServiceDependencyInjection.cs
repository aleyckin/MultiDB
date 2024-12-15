using Microsoft.Extensions.DependencyInjection;
using Services.Profiles;
using Services.Services;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class ServiceDependencyInjection
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICoordinator, Coordinator>();
            services.AddAutoMapper(typeof(UserProfile));
            return services;
        }
    }
}
