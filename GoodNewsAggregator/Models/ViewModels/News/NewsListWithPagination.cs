using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class NewsListWithPagination
    {
        public IEnumerable<NewsList> NewsLists { get; set; }

        public PageInfo Pagination { get; set; }
    }
}
