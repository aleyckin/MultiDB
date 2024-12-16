using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DesignTimeShardDbContextFactory : IDesignTimeDbContextFactory<ShardDbContext>
    {
        public ShardDbContext CreateDbContext(string[] args)
        {
            string connectionString;

            if (args.Length > 0 && args[0] == "shard1")
            {
                connectionString = "Host=shard-db1;Port=5432;Username=postgres;Password=postgres;Database=ShardDb1";
            }
            else
            {
                connectionString = "Host=shard-db2;Port=5432;Username=postgres;Password=postgres;Database=ShardDb2";
            }

            var optionsBuilder = new DbContextOptionsBuilder<ShardDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ShardDbContext(optionsBuilder.Options);
        }
    }
}
