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
    public class GetNewsByRssSourceIdQueryHandler : IRequestHandler<GetNewsByRssSourceIdQuery, IEnumerable<NewsDto>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetNewsByRssSourceIdQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NewsDto>> Handle(GetNewsByRssSourceIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.News
                    .Where(n => n.RssSourceId.Equals(request.Id))
                    .Select(n =>_mapper.Map<NewsDto>(n))
                    .ToListAsync(cancellationToken);
        }
    }
}
