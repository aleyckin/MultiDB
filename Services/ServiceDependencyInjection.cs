using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
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
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<List<string>>(provider =>
            {
                var shardConfiguration = new ShardConfiguration(configuration);
                return shardConfiguration.GetShardConnectionStrings();
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICoordinator, Coordinator>();
            services.AddAutoMapper(typeof(UserProfile));
            return services;
        }
    }
}
