using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.News;
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


        }
    }
}
