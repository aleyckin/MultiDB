using AutoMapper;
using Contracts.Dtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly KeyMappingDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(KeyMappingDbContext dbContext, IMapper mapper, IConfiguration configuration) 
        { 
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<UserDto> GetUserAsync(string login)
        {
            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == login);
            if (userInDb == null)
            {
                throw new Exception("Пользователь с таким логином не найден.");
            }
            return _mapper.Map<UserDto>(userInDb);
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            var users = await _mapper.ProjectTo<UserDto>(_dbContext.Users).ToListAsync();
            if (users.Count == 0)
            {
                throw new Exception("Список пользователей пуст.");
            }
            return users;
        }

        public async Task<UserDto> CreateUserAsync(UserDtoForCreate userDtoForCreate)
        {
            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == userDtoForCreate.Login);
            if (userInDb != null)
            {
                throw new Exception("Пользователь с этим логином уже существует.");
            }

            var user = _mapper.Map<User>(userDtoForCreate);
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteUserAsync(string login)
        {
            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == login);
            if (userInDb == null)
            {
                throw new Exception("Пользователь с таким логином не найден.");
            }

            _dbContext.Users.Remove(userInDb);
            await _dbContext.SaveChangesAsync();
        }

        public string GenerateJwtToken(UserDtoForCreate userDtoForCreate)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDtoForCreate.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", userDtoForCreate.Login.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> ValidateUserCredentials(UserDtoForCreate userDtoForCreate)
        {
            var userInDb = await _dbContext.Users.FirstOrDefaultAsync(x => x.Login == userDtoForCreate.Login);
            if (userInDb == null || userInDb.Password != userDtoForCreate.Password)
            {
                throw new Exception("Неправильный логин или пароль.");
            }
            return GenerateJwtToken(userDtoForCreate);
        }
    }
}
