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
    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, NewsDto>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetNewsByIdQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<NewsDto> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<NewsDto>(
                await _dbContext.News
                    .FirstOrDefaultAsync(n => n.Id.Equals(request.Id), cancellationToken));

        }
    }
}
