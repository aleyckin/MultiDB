using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IShardConfiguration
    {
        List<string> GetShardConnectionStrings();
    }

    public class ShardConfiguration : IShardConfiguration
    {
        private readonly IConfiguration _configuration;

        public ShardConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> GetShardConnectionStrings()
        {
            return new List<string>
        {
            _configuration.GetConnectionString("ShardDb1"),
            _configuration.GetConnectionString("ShardDb2")
        };
        }
    }

}
