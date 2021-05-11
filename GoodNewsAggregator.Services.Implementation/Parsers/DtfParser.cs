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
    public class DtfParser : IWebPageParser
    {
        public string Name { get { return "DTF"; } }

        public string GetAuthor(SyndicationItem item)
        {
            if (item.Authors.Count > 0)
            {
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
            return null;
        }

        public Task<string> GetBody(string url)
        {
            return null;
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

        public Task<string> Parse(string url)
        {
            throw new System.NotImplementedException();
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
