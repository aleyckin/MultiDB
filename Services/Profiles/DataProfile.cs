using AutoMapper;
using Contracts.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Profiles
{
    class DataProfile : Profile
    {
        public DataProfile() 
        {
            CreateMap<Data, DataDto>().ReverseMap();
            CreateMap<Data, DataDtoForCreate>().ReverseMap();
            CreateMap<Data, DataDtoForList>().ReverseMap();
        }
    }
}
