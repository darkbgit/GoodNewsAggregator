using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries.NewsQ
{
    public class GetNewsWithRssSourceNameByIdQuery : IRequest<NewsWithRssNameDto>
    {
        public Guid Id { get; set; }
    }
}
