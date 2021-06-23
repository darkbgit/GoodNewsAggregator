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
    public class AddNewsCommandHandler : IRequestHandler<AddNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public AddNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> Handle(AddNewsCommand request, CancellationToken cancellationToken)
        {
            var news = _mapper.Map<News>(request);
            await _dbContext.News.AddAsync(news, cancellationToken);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
