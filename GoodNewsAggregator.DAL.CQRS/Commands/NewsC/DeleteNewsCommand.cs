using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Commands.NewsC
{
    public class DeleteNewsCommand : IRequest<int>
    {
        public Guid Id { get; set; }
    }
}
