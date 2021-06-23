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
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            var news = _mapper.Map<News>(request);
            _dbContext.News.Update(news);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
