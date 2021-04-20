using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels
{
    public class PageInfo
    {
        

        public PageInfo(int pageSize, int pageNumber, int totalNews)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalNews = totalNews;
        }

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalNews { get; set; }
        public int TotalPages
        {
            get => (int)Math.Ceiling((decimal)TotalNews / PageSize);
        }
    }
}
