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
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.CQRS.CommandHandlers.NewsCH
{
    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public DeleteNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<int> Handle(DeleteNewsCommand request, CancellationToken cancellationToken)
        {
            var news = _dbContext.News.FirstOrDefaultAsync(n => n.Id.Equals(request.Id), cancellationToken).Result;
            _dbContext.News.Remove(news);
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
