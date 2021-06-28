using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers.NewsQH
{
    public class GetNewsPerPageQueryHandler : IRequestHandler<GetNewsPerPageQuery, Tuple<IEnumerable<NewsDto>, int>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetNewsPerPageQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Tuple<IEnumerable<NewsDto>, int>> Handle(GetNewsPerPageQuery request, CancellationToken cancellationToken)
        {
            var news =  _dbContext.News
                    .Where(n => request.Ids.Contains(n.RssSourceId));

            if (request.MinimalRating != null)
            {
                news = news.Where(n => n.Rating >= request.MinimalRating.Value);
            }

            var count = await news.CountAsync(cancellationToken);
            var newsDto = await news
                .Skip((request.PageNumber - 1) * request.NewsPerPage)
                .Select(n => _mapper.Map<NewsDto>(n))
                .ToListAsync(cancellationToken);

            return new Tuple<IEnumerable<NewsDto>, int>(newsDto, count);
        }
    }
}
