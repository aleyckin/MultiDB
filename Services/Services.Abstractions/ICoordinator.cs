using Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Abstractions
{
    public interface ICoordinator
    {
        Task<DataDto> GetDataAsync(Guid userId, Guid dataId);
        Task<DataDto> InsertDataAsync(Guid userId, DataDto dataDto);
        Task RemoveDataAsync(Guid userId, Guid dataId);
        Task<List<DataDto>> GetAllDataAsync();
        Task RemoveAllDataAsync();
    }
}
