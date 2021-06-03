using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.Account;
using GoodNewsAggregator.Models.ViewModels.News;
using GoodNewsAggregator.Models.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Mapping
{
    public class MappingProfile2 : Profile
    {
        public MappingProfile2()
        {

            CreateMap<NewsDto, OneNews>();

            CreateMap<NewsDto, NewsList>();

            CreateMap<RegisterViewModel, User>()
                .ForMember("UserName", opt => opt.MapFrom(r => r.Email));

            CreateMap<CreateUserViewModel, User>()
                .ForMember("UserName", opt => opt.MapFrom(c => c.Email));

            CreateMap<User, EditUserViewModel>();
            CreateMap<EditUserViewModel, User>();

            CreateMap<User, UserViewModel>();
            

        }
    }
}
