using GoodNewsAggregator.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.DAL.Core.Enums;
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.QueryHandlers.NewsQH
{
    public class GetAllExistingNewsWithoutBodyQueryHandler : IRequestHandler<GetAllExistingNewsWithoutBodyQuery, IEnumerable<NewsWithRssNameDto>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;

        public GetAllExistingNewsWithoutBodyQueryHandler(GoodNewsAggregatorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NewsWithRssNameDto>> Handle(GetAllExistingNewsWithoutBodyQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.News
                .Where(n => n.Status < NewsStatus.BodyCompleted)
                .Include(i => i.RssSource)
                .Select(n => new NewsWithRssNameDto
                {
                    Id = n.Id,
                    Url = n.Url,
                    RssSourceId = n.RssSourceId,
                    RssSourceName = n.RssSource.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
