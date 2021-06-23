using GoodNewsAggregator.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers.NewsQH
{
    public class GetAllExistingNewsUrlsQueryHandler : IRequestHandler<GetAllExistingNewsUrlsQuery, IEnumerable<string>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;

        public GetAllExistingNewsUrlsQueryHandler(GoodNewsAggregatorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<string>> Handle(GetAllExistingNewsUrlsQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.News
                .Select(n => n.Url)
                .ToListAsync(cancellationToken);
        }
    }
}
