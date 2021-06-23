using System.Collections.Generic;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ
{
    public class GetAllRssSourceQuery : IRequest<IEnumerable<RssSourceDto>>
    {

    }
}
