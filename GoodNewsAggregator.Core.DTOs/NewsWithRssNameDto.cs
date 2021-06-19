﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Core.DTOs
{
    public class NewsWithRssNameDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSource { get; set; }

        public Guid? RssSourceId { get; set; }
        public string RssSourceName { get; set; }
    }
}
