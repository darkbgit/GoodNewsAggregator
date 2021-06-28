using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface IWebPageParser
    {
        public string Name { get; }

        string GetBody(string url);

        string GetAuthor(SyndicationItem item);

        string GetCategory(SyndicationItem item);

        string GetSummary(SyndicationItem item);

        string GetImageUrl(SyndicationItem item);

        string GetUrl(SyndicationItem item);
    }

    public delegate IWebPageParser ParserResolver(string name);
}
