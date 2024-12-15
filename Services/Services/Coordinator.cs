using AutoMapper;
using Contracts.Dtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class Coordinator : ICoordinator
    {
        private readonly KeyMappingDbContext _dbContext;
        private readonly List<string> _shardConnectionStrings;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<ShardDbContext> _dbContextFactory;

        public Coordinator(KeyMappingDbContext dbContext, List<string> shardConnectionStrings, IMapper mapper, IDbContextFactory<ShardDbContext> dbContextFactory) 
        { 
            _dbContext = dbContext;
            _shardConnectionStrings = shardConnectionStrings;
            _mapper = mapper;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<string> GetOrCreateKeyForUserAsync(Guid userId)
        {
            var mapping = await _dbContext.KeyMappings.FindAsync(userId);
            if (mapping != null)
            {
                return mapping.Key;
            }

            string newKey = Guid.NewGuid().ToString();
            _dbContext.KeyMappings.Add(new KeyMapping { UserId = userId, Key = newKey });
            await _dbContext.SaveChangesAsync();

            return newKey;
        }

        public async Task<DataDto> GetDataAsync(Guid userId, Guid dataId)
        {
            string key = await GetOrCreateKeyForUserAsync(userId);
            int shardIndex = GetShardIndex(key);
            var shardDbContext = CreateShardDbContext(_shardConnectionStrings[shardIndex]);
            return await GetDataFromShardAsync(shardDbContext, dataId);
        }

        public async Task<DataDto> InsertDataAsync(Guid userId, DataDto dataDto)
        {
            string key = await GetOrCreateKeyForUserAsync(userId);
            int shardIndex = GetShardIndex(key);
            var shardDbContext = CreateShardDbContext(_shardConnectionStrings[shardIndex]);
            return await InsertDataInShardAsync(shardDbContext, dataDto);
        }

        public async Task RemoveDataAsync(Guid userId, Guid dataId)
        {
            string key = await GetOrCreateKeyForUserAsync(userId);
            int shardIndex = GetShardIndex(key);
            var shardDbContext = CreateShardDbContext(_shardConnectionStrings[shardIndex]);
            await RemoveDataFromShardAsync(shardDbContext, dataId);
        }

        public async Task<List<DataDto>> GetAllDataAsync()
        {
            var tasks = _shardConnectionStrings.Select(async connectionString =>
            {
                var shardDbContext = CreateShardDbContext(connectionString);
                return await GetAllDataFromShardAsync(shardDbContext);
            });

            var dataFromAllShards = await Task.WhenAll(tasks);

            return dataFromAllShards.SelectMany(x => x).ToList();
        }

        public async Task RemoveAllDataAsync()
        {
            var tasks = _shardConnectionStrings.Select(async connectionStrings =>
            {
                var shardDbContext = CreateShardDbContext(connectionStrings);
                await RemoveAllDataFromShardAsync(shardDbContext);
            });
            await Task.WhenAll(tasks);
        }

        public int GetShardIndex(string key)
        {
            return Math.Abs(key.GetHashCode()) % _shardConnectionStrings.Count;
        }

        private async Task<DataDto> GetDataFromShardAsync(ShardDbContext shardDbContext, Guid dataId)
        {
            var data = await shardDbContext.Data.FirstOrDefaultAsync(x => x.Id == dataId);
            if (data == null)
            {
                throw new Exception("Данных с этим id не найдено.");
            }
            return _mapper.Map<DataDto>(data);
        }

        private async Task RemoveDataFromShardAsync(ShardDbContext shardDbContext, Guid dataId)
        {
            var data = await shardDbContext.Data.FirstOrDefaultAsync(x => x.Id == dataId);
            if (data == null)
            {
                throw new Exception("Данных с этим id не найдено.");
            }
            shardDbContext.Remove(data);
            await shardDbContext.SaveChangesAsync();
        }

        private async Task<List<DataDto>> GetAllDataFromShardAsync(ShardDbContext shardDbContext)
        {
            var data = await _mapper.ProjectTo<DataDto>(shardDbContext.Data).ToListAsync();
            if (data == null)
            {
                throw new Exception("Данных в шарде не найдено.");
            }
            return data;
        }

        private async Task RemoveAllDataFromShardAsync(ShardDbContext shardDbContext)
        {
            var data = await shardDbContext.Data.ToListAsync();
            if (data == null)
            {
                throw new Exception("Данных в шарде не найдено.");
            }
            shardDbContext.Remove(data);
            await shardDbContext.SaveChangesAsync();
        }

        private async Task<DataDto> InsertDataInShardAsync(ShardDbContext shardDbContext, DataDto dataDto)
        {
            var data = _mapper.Map<Data>(dataDto);
            shardDbContext.Add(data);
            await shardDbContext.SaveChangesAsync();
            return dataDto;
        }

        private ShardDbContext CreateShardDbContext(string connectionString)
        {
            var options = new DbContextOptionsBuilder<ShardDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new ShardDbContext(options, connectionString);
        }
    }
}
