using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IShardDbContextFactory
    {
        ShardDbContext Create(string connectionString);
    }

    public class ShardDbContextFactory : IShardDbContextFactory
    {
        public ShardDbContext Create(string connectionString)
        {
            var options = new DbContextOptionsBuilder<ShardDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ShardDbContext(options);
        }
    }
}
