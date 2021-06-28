using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class TutByParser : IWebPageParser
    {
        public string Name => "Tut.by";

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
                            Author = GetAuthor(syndicationItem),
                            Category = GetCategory(syndicationItem),
                            Url =
                                syndicationItem.Links.FirstOrDefault(sl => sl.RelationshipType.Equals("alternate"))?.Uri
                                    .AbsoluteUri,
                            ImageUrl =
                                syndicationItem.Links.FirstOrDefault(sl =>
                                    sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri,
                            ShortNewsFromRssSource = GetSummary(syndicationItem),
                            Title = syndicationItem.Title.Text,
                            PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime()
                        };
                        news.Add(newsDto);
                    }
                }
            }
            return news;
        }

        public string GetSummary(SyndicationItem item)
        {
            return Regex.Replace(item.Summary.Text.Trim(), @"<.*?>", "");
        }

        public string GetCategory(SyndicationItem item)
        {
            if (item.Categories.Count == 0) return null;

            string category = "";
            for (int i = 0; i < item.Categories.Count; i++)
            {
                category += item.Categories[i].Name;
                if (i > 0 && i != item.Categories.Count - 1)
                {
                    category += ", ";
                }
            }
            return null;
        }

        public string GetAuthor(SyndicationItem item)
        {
            if (item.Authors.Count == 0) return null;

            string author = "";
            for (int i=0; i<item.Authors.Count; i++)
            {
                author += item.Authors[i].Name;
                if (i > 0 && i != item.Authors.Count - 1)
                {
                    author += ", ";
                }
            }
            return author;
        }

        public string GetBody(string url)
        {
            return null;
        }



        public string GetImageUrl(SyndicationItem item)
        {
            return item.Links.FirstOrDefault(sl =>
                                    sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri;
        }

        public string GetUrl(SyndicationItem item)
        {
            return item.Links.FirstOrDefault(sl => sl.RelationshipType.Equals("alternate"))?.Uri
                                    .AbsoluteUri;
        }
    }
}
