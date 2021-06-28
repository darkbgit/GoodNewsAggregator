using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using HtmlAgilityPack;

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class S13Parser : IWebPageParser
    {
        public string Name => "S13";

        public string GetAuthor(SyndicationItem item)
        {
            return null;
        }

        public  string GetBody(string url)
        {
            var web = new HtmlWeb();
            var doc = web.LoadFromWebAsync(url).Result;


            HtmlNode node = doc.DocumentNode.SelectSingleNode("//ul[@class='cols top']/li/div[@class='content']")
                          ?? doc.DocumentNode.SelectSingleNode("//ul[@class='cols newsmore']/li/div[@class='content']");
            if (node == null) return null;

            node.SelectNodes("//div[@class='news-reference']|//div")
                .ToList()
                .ForEach(n => n.Remove());

            return node.OuterHtml;
        }

        public string GetCategory(SyndicationItem item)
        {
            if (item.Categories.Count <= 0) return null;

            string category = "";
            for (int i = 0; i < item.Categories.Count; i++)
            {
                category += item.Categories[i].Name;
                if (i == item.Categories.Count - 1)
                {
                    continue;
                }
                category += ", ";
            }
            return category;
        }

        public string GetImageUrl(SyndicationItem item)
        {
            return null;
        }

        public string GetSummary(SyndicationItem item)
        {
            return item.Summary.Text.Trim();
        }

        public string GetUrl(SyndicationItem item)
        {
            return item.Id;
        }
    }
}
