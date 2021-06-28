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
    public class GetNewsWithRssSourceNameByIdQueryHandler : IRequestHandler<GetNewsWithRssSourceNameByIdQuery, NewsWithRssNameDto>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetNewsWithRssSourceNameByIdQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<NewsWithRssNameDto> Handle(GetNewsWithRssSourceNameByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.News
                    .Where(n => n.Id.Equals(request.Id))
                    .Include(n => n.RssSource)
                    .Select(n =>_mapper.Map<NewsWithRssNameDto>(n))
                    .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
