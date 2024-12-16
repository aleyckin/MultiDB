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
        Task<DataDtoForList> GetDataAsync(Guid userId, Guid dataId);
        Task<List<DataDto>> GetAllDataForUserAsync(Guid userId);
        Task<List<DataDtoForList>> GetAllDataAsync();
        Task<DataDtoForCreate> InsertDataAsync(DataDtoForCreate dataDto);
        Task RemoveDataAsync(Guid userId, Guid dataId);
        Task RemoveAllDataAsync();
    }
}
