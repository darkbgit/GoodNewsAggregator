using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.CQRS.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers
{
    public class GetRssSourceByNameAndUrlQueryHandler :IRequestHandler<GetRssSourceByNameAndUrlQuery, IEnumerable<RssSourceDto>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetRssSourceByNameAndUrlQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RssSourceDto>> Handle(GetRssSourceByNameAndUrlQuery request, CancellationToken cancellationToken)
        {
            return 
            (await _dbContext.RssSources
                    .Where(source => source.Name.Equals(request.Name) && source.Url.Equals(request.Url))
                    .ToListAsync(cancellationToken))
            .Select(source => _mapper.Map<RssSourceDto>(source));
        }
    }
}
