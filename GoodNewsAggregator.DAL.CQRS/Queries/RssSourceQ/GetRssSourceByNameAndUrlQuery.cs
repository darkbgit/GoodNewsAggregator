using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries
{
    public class GetRssSourceByNameAndUrlQuery :IRequest<IEnumerable<RssSourceDto>>
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public GetRssSourceByNameAndUrlQuery(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
