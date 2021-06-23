using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries.NewsQ
{
    public class GetAllExistingNewsUrlsQuery : IRequest<IEnumerable<string>>
    {
    }
}
