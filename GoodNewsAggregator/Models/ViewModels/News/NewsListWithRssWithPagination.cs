using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class NewsListWithRssWithPagination
    {
        public IEnumerable<NewsList> NewsLists { get; set; }

        public IEnumerable<RssList> RssList { get; set; }

        public Paging Pagination { get; set; }
    }
}
