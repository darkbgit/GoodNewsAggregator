using GoodNewsAggregator.DAL.Core.Enums;
using System;

namespace GoodNewsAggregator.Core.DTOs
{
    public class NewsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSource { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public NewsStatus Status { get; set; }

        public Guid RssSourceId { get; set; }
    }
}
