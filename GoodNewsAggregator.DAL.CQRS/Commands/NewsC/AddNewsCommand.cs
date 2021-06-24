using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Commands.NewsC
{
    public class AddNewsCommand : IRequest<int>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSource { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }
        public string Author { get; set; }

        public Guid RssSourceId { get; set; }
    }
}
