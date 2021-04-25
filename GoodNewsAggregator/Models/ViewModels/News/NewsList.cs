﻿using GoodNewsAggregator.Core.DTOs;
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

        public string ShortNewsFromRssSourse { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicationDate { get; set; }

        //public Guid RssSourceId { get; set; }

        
    }
}
