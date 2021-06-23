using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers.RssSourceQH
{
    public class GetRssSourceByIdQueryHandler : IRequestHandler<GetRssSourceByIdQuery, RssSourceDto>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetRssSourceByIdQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<RssSourceDto> Handle(GetRssSourceByIdQuery request, CancellationToken cancellationToken)
        {
            return _mapper.Map<RssSourceDto>(
                await _dbContext.RssSources
                    .FirstOrDefaultAsync(source => source.Id.Equals(request.Id), cancellationToken));
        }
    }
}
