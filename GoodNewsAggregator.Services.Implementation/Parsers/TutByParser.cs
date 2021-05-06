using System.Collections.Generic;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class TutByParser : IWebPageParser
    {
        public Task<string> Parse(string url)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<NewsDto>> ParseRss(RssSourceDto rss)
        {
            throw new System.NotImplementedException();
        }
    }
}
