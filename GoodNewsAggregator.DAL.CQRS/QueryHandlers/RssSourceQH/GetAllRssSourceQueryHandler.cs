using System.Collections.Generic;
using System.Linq;
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
    public class GetAllRssSourceQueryHandler : IRequestHandler<GetAllRssSourceQuery, IEnumerable<RssSourceDto>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetAllRssSourceQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RssSourceDto>> Handle(GetAllRssSourceQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.RssSources
                .Select(s => _mapper.Map<RssSourceDto>(s))
                .ToListAsync(cancellationToken); 
        }
    }
}
