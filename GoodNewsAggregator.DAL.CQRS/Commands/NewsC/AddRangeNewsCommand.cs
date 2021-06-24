using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Commands.NewsC
{
    public class AddRangeNewsCommand : IRequest<int>
    {
        public IEnumerable<NewsDto> News { get; set; }
    }
}
