using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class GetPureNewsTextByIdQueryHandler : IRequestHandler<GetPureNewsTextByIdQuery, string>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public GetPureNewsTextByIdQueryHandler(GoodNewsAggregatorContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<string> Handle(GetPureNewsTextByIdQuery request, CancellationToken cancellationToken)
        {
            var body = (await _dbContext.News
                    .FirstOrDefaultAsync(n => n.Id.Equals(request.Id), cancellationToken))
                .Body;

            string pattern = @"(?:<).*?(?:>)";
            string target = " ";
            var rgx = new Regex(pattern);
            var result = rgx.Replace(body, target);
            return result;
        }
    }
}
