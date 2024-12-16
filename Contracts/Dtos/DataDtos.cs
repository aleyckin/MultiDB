using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos
{
    public record DataDto(string Description, DateTime CreatedDate);
    public record DataDtoForCreate(string Description, Guid userId);
    public record DataDtoForList(Guid userId, string Description, DateTime CreatedDate, string ShardName);
}
