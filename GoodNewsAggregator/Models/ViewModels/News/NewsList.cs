using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class NewsList
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public string ShortNewsFromRssSource { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }


        //public Guid RssSourceId { get; set; }


    }
}
