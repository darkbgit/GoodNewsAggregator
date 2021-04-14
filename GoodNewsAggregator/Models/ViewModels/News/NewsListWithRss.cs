using GoodNewsAggregator.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.News
{
    public class NewsListWithRss
    {
        public IEnumerable<NewsList> NewsLists { get; set; }

        public IEnumerable<RssSourseDto> RssSourses { get; set; }
    }
}
