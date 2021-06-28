using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using HtmlAgilityPack;
using Serilog;

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class OnlinerParser : IWebPageParser
    {
        public string Name => "Onliner";
        
        public string GetSummary(SyndicationItem item)
        {
            return Regex.Replace(item.Summary.Text.Trim(), @"<.*?>", "");
        }

        public string GetCategory(SyndicationItem item)
        {
            if (item.Categories.Count == 0) return null;
            
            string category = "";
            for (int i=0; i<item.Categories.Count; i++)
            {
                category += item.Categories[i].Name;
                if (i > 0 && i != item.Categories.Count - 1)
                {
                    category += ", ";
                }
            }
            return category;
        }

        public string GetAuthor(SyndicationItem item)
        {
            if (item.Authors.Count <= 0) return null;
            
            string author = "";
            foreach (var a in item.Authors)
            {
                author += a.Name;
            }
            return author;
        }

        public string GetBody(string url)
        {
            var web = new HtmlWeb();
            var doc =  web.LoadFromWebAsync(url).Result;

            var node = doc.DocumentNode.SelectSingleNode("//div[@class='news-text']");

            if (node == null) return null;

            node.SelectNodes("//div[@class='news-reference']|//div[@class='news-widget news-widget_special']|//script|//p[@style='text-align: right;']")
                .ToList()
                .ForEach(n => n.Remove());
            //node.RemoveChild(newsReference);
            //node.RemoveChildren(node.SelectNodes("//div[@class='news-widget']"));
            //node.RemoveChildren(node.SelectNodes("//scripts"));

            var nn = node.SelectSingleNode("//div[@class='news-media news-media_condensed']");

            if (node.SelectSingleNode("//div[@class='news-media news-media_condensed']") != null)
            {

                if (node.SelectSingleNode("//div[@class='news-media news-media_condensed']")
                    .NextSibling
                    .NextSibling
                    .HasAttributes &&
                    node.SelectSingleNode("//div[@class='news-media news-media_condensed']")
                    .NextSibling
                    .NextSibling.
                    Attributes[0].Name == "class" &&
                    node.SelectSingleNode("//div[@class='news-media news-media_condensed']")
                    .NextSibling
                    .NextSibling
                    .Attributes[0].Value == "news-widget")
                {
                    node.SelectNodes("//div[@class='news-media news-media_condensed']|//div[@class='news-widget']")
                    .ToList()
                    .ForEach(n => n.Remove());
                }
            }

            node.SelectNodes("div[@class='news-widget']")
                    .ToList()
                    .ForEach(n => n.Remove());

            return node.OuterHtml;
        }


        public string GetImageUrl(SyndicationItem item)
        {
            var link = item.Links.FirstOrDefault(sl =>
                                    sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri;
            if (link != null) return link;

            var match = Regex.Match(item.Summary.Text, "(?:<img src=\")(.*?)(?:\")");
            link = match.Groups[1].Value;
            return link;
        }

        public string GetUrl(SyndicationItem item)
        {
            return item.Id;
        }
    }
}
