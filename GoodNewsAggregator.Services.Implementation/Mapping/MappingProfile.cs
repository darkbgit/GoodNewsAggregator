using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.CQRS.Commands.NewsC;

namespace GoodNewsAggregator.Services.Implementation.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<News, NewsDto>();
            CreateMap<NewsDto, News>();

            CreateMap<RssSource, RssSourceDto>();
            CreateMap<RssSourceDto, RssSource>();
            //CreateMap<OneNews, NewsDto>();

            CreateMap<Comment, CommentDto>();
            CreateMap<CommentDto, Comment>();

            CreateMap<News, NewsWithRssNameDto>();

            CreateMap<AddNewsCommand, News>();
            CreateMap<UpdateNewsCommand, News>();

            CreateMap<NewsDto, AddNewsCommand>();
            CreateMap<NewsDto, UpdateNewsCommand>();
            
        }
    }
}
