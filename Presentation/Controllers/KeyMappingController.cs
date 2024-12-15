using Contracts.Dtos;
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
    [ApiController]
    [Route("api/keymappings")]
    public class KeyMappingController
    {
        private readonly ICoordinator _coordinator;
        
        public KeyMappingController(ICoordinator coordinator) { _coordinator = coordinator; }

        [HttpGet("{userId:guid}/{dataId:guid}")]
        public async Task<ActionResult<DataDto>> GetData(Guid userId, Guid dataId)
        {
            return await _coordinator.GetDataAsync(userId, dataId);
        }

        [HttpGet]
        public async Task<ActionResult<List<DataDto>>> GetAllData()
        {
            return await _coordinator.GetAllDataAsync();
        }

        [HttpPost("{userId:guid}")]
        public async Task<ActionResult<DataDto>> CreateData(Guid userId, [FromBody] DataDto dataDto)
        {
            return await _coordinator.InsertDataAsync(userId, dataDto);
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
