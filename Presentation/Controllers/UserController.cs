using Contracts.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{login}")]
        public async Task<ActionResult<UserDto>> GetUser(string login)
        {
            return await _userService.GetUserAsync(login);
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            return await _userService.GetUsersAsync();
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserDtoForCreate userDtoForCreate)
        {
            var userDto = await _userService.CreateUserAsync(userDtoForCreate);
            return userDto;
        }

        [HttpDelete("{login}")]
        public async Task DeleteUser(string login)
        {
            await _userService.DeleteUserAsync(login);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDtoForCreate userDtoForCreate)
        {
            return await _userService.ValidateUserCredentials(userDtoForCreate);
        }
    }
}
