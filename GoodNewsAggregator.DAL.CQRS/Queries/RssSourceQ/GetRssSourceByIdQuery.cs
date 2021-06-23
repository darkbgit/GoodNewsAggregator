using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ
{
    public class GetRssSourceByIdQuery : IRequest<RssSourceDto>
    {
        public Guid Id { get; set; }

        public GetRssSourceByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
