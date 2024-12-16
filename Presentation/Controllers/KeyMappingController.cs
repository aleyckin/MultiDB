using Contracts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/keymappings")]
    public class KeyMappingController
    {
        private readonly ICoordinator _coordinator;
        
        public KeyMappingController(ICoordinator coordinator) { _coordinator = coordinator; }

        [HttpGet("{userId:guid}/{dataId:guid}")]
        public async Task<ActionResult<DataDtoForList>> GetData(Guid userId, Guid dataId)
        {
            return await _coordinator.GetDataAsync(userId, dataId);
        }

        [HttpGet]
        public async Task<ActionResult<List<DataDtoForList>>> GetAllData()
        {
            return await _coordinator.GetAllDataAsync();
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<List<DataDto>>> GetAllDataForUser(Guid userId)
        {
            return await _coordinator.GetAllDataForUserAsync(userId);
        }

        [HttpPost]
        public async Task<ActionResult<DataDtoForCreate>> CreateData([FromBody] DataDtoForCreate dataDto)
        {
            return await _coordinator.InsertDataAsync(dataDto);
        }

        [HttpDelete("{userId:guid}/{dataId:guid}")]
        public async Task DeleteData(Guid userId, Guid dataId)
        {
            await _coordinator.RemoveDataAsync(userId, dataId);
        }

        [HttpDelete]
        public async Task DeleteAllData()
        {
            await _coordinator.RemoveAllDataAsync();
        }

    }
}
