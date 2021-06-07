using GoodNewsAggregator.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels
{
    public class PageInfo
    {
        

        public PageInfo(int pageNumber, int totalNews, int pageSize = Constants.NEWS_PER_PAGE)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalNews = totalNews;
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalNews { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalNews / PageSize);
    }
}
