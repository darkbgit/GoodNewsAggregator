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

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class TJournalParser : IWebPageParser
    {
        public string Name => "TJournal";

        public string GetAuthor(SyndicationItem item)
        {
            if (item.Authors.Count <= 0) return null;

            string author = "";
            for (int i = 0; i < item.Authors.Count; i++)
            {
                author += item.Authors[i].Email;
                if (i > 0 && i != item.Authors.Count - 1)
                {
                    author += ", ";
                }
            }
            return author;
        }

        public string GetBody(string url)
        {
            var web = new HtmlWeb();
            var doc =  web.LoadFromWebAsync(url).Result;

            var node = doc.DocumentNode.SelectSingleNode("//div[@class='l-entry__content']");

            if (node == null) return null;

            node.SelectNodes("//div[@class='content-header__info']|//div[@class='content-header__spacer']|//div[@class='l-hidden entry_data']|//div[@class='subsite-card-entry']|//script|//div[@class='content-info content-info--full l-island-a;']")
                .ToList()
                .ForEach(n => n.Remove());

            return node.OuterHtml;
        }

        public string GetCategory(SyndicationItem item)
        {
            return null;
        }

        public string GetImageUrl(SyndicationItem item)
        {
            return item.Links.FirstOrDefault(sl =>
                                    sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri;
        }

        public string GetSummary(SyndicationItem item)
        {
            return Regex.Replace(item.Summary.Text.Trim(), @"<.*?>", "");
        }

        public string GetUrl(SyndicationItem item)
        {
            return item.Links.FirstOrDefault(sl => sl.RelationshipType.Equals("alternate"))?.Uri
                                    .AbsoluteUri;
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
                            Title = syndicationItem.Title.Text,
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
