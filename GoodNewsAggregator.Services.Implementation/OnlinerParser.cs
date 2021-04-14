using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.Services.Interfaces;
using HtmlAgilityPack;

namespace GoodNewsAggregator.Services.Implementation
{
    public class OnlinerParser : IWebPageParser
    {
        public async Task<string> Parse(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            var node = doc.DocumentNode.SelectSingleNode("//div[@class='news-text']");

            node.SelectNodes("//div[@class='news-reference']|//div[@class='news-widget news-widget_special']|//script|//p[@style='text-align: right;']")
                .ToList()
                .ForEach(n=>n.Remove());
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
    }
}
