using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Dtos
{
    public record UserDtoForCreate(string Login, string Password); 
    public record UserDto(Guid Id, string Login);
}
