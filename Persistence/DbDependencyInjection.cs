using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public static class DbDependencyInjection
    {
        public static IServiceCollection AddDbDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<KeyMappingDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("KeyMappingDb")));

            services.AddSingleton<IShardDbContextFactory, ShardDbContextFactory>();
            services.AddSingleton<IShardConfiguration, ShardConfiguration>();
            return services;
        }
    }
}
