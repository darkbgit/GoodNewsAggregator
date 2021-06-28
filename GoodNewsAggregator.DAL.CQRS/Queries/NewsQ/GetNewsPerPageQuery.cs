using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries.NewsQ
{
    public class GetNewsPerPageQuery : IRequest<Tuple<IEnumerable<NewsDto>, int>>
    {
        public Guid[] Ids { get; set; }
        public int PageNumber { get; set; }
        public int NewsPerPage { get; set; }
        public string SortOrder { get; set; }
        public double? MinimalRating { get; set; }
    }
}
