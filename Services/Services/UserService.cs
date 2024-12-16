using AutoMapper;
using Contracts.Dtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Services.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly KeyMappingDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserService(KeyMappingDbContext dbContext, IMapper mapper) 
        { 
            _dbContext = dbContext;
            _mapper = mapper;
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
    }
}
