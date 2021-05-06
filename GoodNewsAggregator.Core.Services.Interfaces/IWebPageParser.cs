using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface IWebPageParser
    {
        Task<string> Parse(string url);

        Task<IEnumerable<NewsDto>> ParseRss(RssSourceDto rss);
    }
}
