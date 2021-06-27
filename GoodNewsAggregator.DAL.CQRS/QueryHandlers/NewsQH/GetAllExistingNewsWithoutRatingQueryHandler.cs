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
    public class GetAllExistingNewsWithoutRatingQueryHandler : IRequestHandler<GetAllExistingNewsWithoutRatingQuery, IEnumerable<NewsDto>>
    {
        private readonly GoodNewsAggregatorContext _dbContext;

        public GetAllExistingNewsWithoutRatingQueryHandler(GoodNewsAggregatorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<NewsDto>> Handle(GetAllExistingNewsWithoutRatingQuery request,
            CancellationToken cancellationToken)
        {
            return await _dbContext.News
                .Where(n => n.Status == NewsStatus.BodyCompleted)
                .OrderBy(n => n.PublicationDate)
                .Take(30)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Url = n.Url,
                    Body = n.Body
                })
                .ToListAsync(cancellationToken);
        }
    }
}
