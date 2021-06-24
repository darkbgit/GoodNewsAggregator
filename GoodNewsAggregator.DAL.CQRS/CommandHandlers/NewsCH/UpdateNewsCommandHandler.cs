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
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, int>
    {
        private readonly GoodNewsAggregatorContext _dbContext;
        private readonly IMapper _mapper;

        public UpdateNewsCommandHandler(GoodNewsAggregatorContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateNewsCommand command, CancellationToken cancellationToken)
        {
            var news = await _dbContext.News
                .Where(n => n.Id.Equals(command.Id))
                .FirstOrDefaultAsync(cancellationToken);
            if (news == null) return default;
            _dbContext.News.Update(_mapper.Map(command, news));
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
