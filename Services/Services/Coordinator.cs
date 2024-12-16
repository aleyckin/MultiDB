using AutoMapper;
using Contracts.Dtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly IShardDbContextFactory _shardDbContextFactory;

        public Coordinator(KeyMappingDbContext dbContext, List<string> shardConnectionStrings, IMapper mapper, IShardDbContextFactory shardDbContextFactory) 
        { 
            _dbContext = dbContext;
            _shardConnectionStrings = shardConnectionStrings;
            _mapper = mapper;
            _shardDbContextFactory = shardDbContextFactory;
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

        public async Task<DataDtoForList> GetDataAsync(Guid userId, Guid dataId)
        {
            string key = await GetOrCreateKeyForUserAsync(userId);
            int shardIndex = GetShardIndex(key);
            var shardDbContext = CreateShardDbContext(_shardConnectionStrings[shardIndex]);
            return await GetDataFromShardAsync(shardDbContext, dataId);
        }

        public async Task<DataDtoForCreate> InsertDataAsync(DataDtoForCreate dataDto)
        {
            string key = await GetOrCreateKeyForUserAsync(dataDto.userId);
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

        public async Task<List<DataDtoForList>> GetAllDataAsync()
        {
            var tasks = _shardConnectionStrings.Select(async (connectionString, index) =>
            {
                var shardDbContext = CreateShardDbContext(connectionString);
                string shardName = $"Shard-{index + 1}";
                return await GetAllDataFromShardAsync(shardDbContext, shardName);
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

        private async Task<DataDtoForList> GetDataFromShardAsync(ShardDbContext shardDbContext, Guid dataId)
        {
            var data = await shardDbContext.Data.FirstOrDefaultAsync(x => x.Id == dataId);
            if (data == null)
            {
                throw new Exception("Данных с этим id не найдено.");
            }
            return _mapper.Map<DataDtoForList>(data);
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

        private async Task<List<DataDtoForList>> GetAllDataFromShardAsync(ShardDbContext shardDbContext, string shardName)
        {
            var data = await shardDbContext.Data.ToListAsync();
            var updatedData = data.Select(d => new DataDtoForList(d.UserId, d.Description, d.CreatedDate, shardName)).ToList();
            return updatedData;
        }

        public async Task<List<DataDto>> GetAllDataForUserAsync(Guid userId)
        {
            string key = await GetOrCreateKeyForUserAsync(userId);
            int shardIndex = GetShardIndex(key);
            var shardDbContext = CreateShardDbContext(_shardConnectionStrings[shardIndex]);
            return await _mapper.ProjectTo<DataDto>(shardDbContext.Data.Where(x => x.UserId == userId)).ToListAsync();
        }

        private async Task RemoveAllDataFromShardAsync(ShardDbContext shardDbContext)
        {
            var data = await shardDbContext.Data.ToListAsync();
            shardDbContext.RemoveRange(data);
            await shardDbContext.SaveChangesAsync();
        }

        private async Task<DataDtoForCreate> InsertDataInShardAsync(ShardDbContext shardDbContext, DataDtoForCreate dataDto)
        {
            var data = _mapper.Map<Data>(dataDto);
            shardDbContext.Add(data);
            await shardDbContext.SaveChangesAsync();
            return dataDto;
        }

        private ShardDbContext CreateShardDbContext(string connectionString)
        {
            return _shardDbContextFactory.Create(connectionString);
        }
    }
}
