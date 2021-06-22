using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class NewsWithCommentsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSource { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicationDate { get; set; }

        public Guid? RssSourceId { get; set; }
        public string RssSourceName { get; set; }

        public int TotalComments { get; set; }
        //public IEnumerable<CommentDto> Comments { get; set; }
    }
}
