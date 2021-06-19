using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;

namespace GoodNewsAggregator.Services.Implementation.Parsers
{
    public class S13Parser : IWebPageParser
    {
        public string Name => "S13";

        public string GetAuthor(SyndicationItem item)
        {
            return null;
        }

        public Task<string> GetBody(string url)
        {
            return null;
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
