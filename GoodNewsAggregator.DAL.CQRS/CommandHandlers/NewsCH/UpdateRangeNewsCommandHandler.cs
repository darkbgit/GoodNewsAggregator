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
    public class UpdateRangeNewsCommandHandler : IRequestHandler<UpdateRangeNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateRangeNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateRangeNewsCommand command, CancellationToken cancellationToken)
        {
            var news = command.News
                .Select(n => _mapper.Map<News>(n));
            _dbContext.News.UpdateRange(news);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
