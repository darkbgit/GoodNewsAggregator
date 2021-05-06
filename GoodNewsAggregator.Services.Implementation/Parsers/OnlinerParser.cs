using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
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

        public async Task<IEnumerable<NewsDto>> ParseRss(RssSourceDto rss)
        {
            var news = new List<NewsDto>();
            using (var reader = XmlReader.Create(rss.Url))
            {
                var feed = SyndicationFeed.Load(reader);
                reader.Close();
                if (feed.Items.Any())
                {
                    //var currentNewsUrls = await _unitOfWork.News
                    //    .Get()
                    //    .Select(n => n.Url)
                    //    .ToListAsync();
                    foreach (var syndicationItem in feed.Items)
                    {
                        var newsDto = new NewsDto
                        {
                            Id = Guid.NewGuid(),
                            RssSourceId = rss.Id,
                            Author = syndicationItem.Authors?[0]?.Email,
                            Url =
                                syndicationItem.Links.FirstOrDefault(sl => sl.RelationshipType.Equals("alternate"))?.Uri
                                    .AbsoluteUri,
                            ImageUrl =
                                syndicationItem.Links.FirstOrDefault(sl =>
                                    sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri,
                            ShortNewsFromRssSource = syndicationItem.Summary.Text.Trim(),

                            //newsDto.Url = syndicationItem.Id;
                            Title = syndicationItem.Title.Text,
                            //ShortNewsFromRssSource = syndicationItem.Summary.Text
                            //newsDto.ShortNewsFromRssSource =
                            //    GetPureShortNewsFromRssSource(syndicationItem.Summary.Text);
                            //newsDto.ImageUrl = GetNewsImageUrlFromRssSource(syndicationItem.Summary.Text);
                            PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime()

                        };
                        news.Add(newsDto);
                    }
                }
            }

            return news;
        }
    }
}
