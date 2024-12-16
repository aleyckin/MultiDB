using Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Abstractions
{
    public interface IUserService
    {
        Task<UserDto> GetUserAsync(string login);
        Task<List<UserDto>> GetUsersAsync();
        Task<UserDto> CreateUserAsync(UserDtoForCreate userDto);
        Task<string> ValidateUserCredentials(UserDtoForCreate userDtoForCreate);
        Task DeleteUserAsync(string login);
    }
}
