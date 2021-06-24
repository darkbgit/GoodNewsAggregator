using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.CQRS.Commands.NewsC;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.CommandHandlers.NewsCH
{
    public class AddRangeNewsCommandHandler : IRequestHandler<AddRangeNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public AddRangeNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddRangeNewsCommand command, CancellationToken cancellationToken)
        {
            var news = command.News
                .Select(n => _mapper.Map<News>(n));
            await _dbContext.News.AddRangeAsync(news, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
