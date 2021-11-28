using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using AutoMapper;
using GoodNewsAggregator.DAL.Core.Enums;
using GoodNewsAggregator.DAL.CQRS.Commands.NewsC;
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GoodNewsAggregator.Services.Implementation
{
    public class NewsCqsService : INewsService
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IWebPageParser> _parsers;

        public NewsCqsService(IMapper mapper,
            IEnumerable<IWebPageParser> parsers,
            IMediator mediator)
        {
            _mapper = mapper;
            _parsers = parsers;
            _mediator = mediator;
        }

        public async Task<IEnumerable<NewsDto>> GetNewsByRssSourceIds(Guid[] ids)
        {
            IEnumerable<NewsDto> news = new List<NewsDto>();
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    news = news.Concat(await _mediator.Send(new GetNewsByRssSourceIdQuery() {Id = id}));
                }
            }
            else
            {
                news = await _mediator.Send(new GetAllNewsQuery());
            }
            return news;
        }

        public async Task<Tuple<IEnumerable<NewsDto>, int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder,
            double? minimalRating = null)
        {
            return await _mediator.Send(new GetNewsPerPageQuery
            {
                Ids = rssIds,
                NewsPerPage = newsPerPage,
                PageNumber = pageNumber,
                SortOrder = sortOrder,
                MinimalRating = minimalRating
            });
        }

        public async Task Aggregate()
        {
            var news = new ConcurrentBag<NewsDto>();
            var rssSources = await _mediator.Send(new GetAllRssSourceQuery());
            var currentNewsUrls = await _mediator.Send(new GetAllExistingNewsUrlsQuery());

            Parallel.ForEach(rssSources, new ParallelOptions{MaxDegreeOfParallelism = 1}, (rssSource) =>
            {
                var newsFromRss = GetNewsInfoFromRssSource(rssSource, currentNewsUrls);
                if (newsFromRss == null) return;
                foreach (var n in newsFromRss)
                {
                    news.Add(n);
                }
            });

            if (news.Any()) await AddRange(news);
        }

        public async Task GetBodies()
        {
            var newsWithoutBody = new ConcurrentBag<NewsWithRssNameDto>(await _mediator
                .Send(new GetAllExistingNewsWithoutBodyQuery()));

            foreach (var news in newsWithoutBody)
            {
                try
                {
                    var parser = _parsers.Single(p => p.Name.Equals(news.RssSourceName));
                    var body =  parser.GetBody(news.Url);
                    if (body == null) continue;
                    news.Body = body;
                    news.Status = NewsStatus.BodyCompleted;
                }
                catch 
                {
                    Log.Error($"Cant take body from news url {news.Url}");
                }
                
            }

            var updatedNews = newsWithoutBody
                .Where(n => n.Status == NewsStatus.BodyCompleted)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Body = n.Body,
                    Status = n.Status
                })
                .ToList();

            if (newsWithoutBody.Any()) await UpdateRange(updatedNews);
        }

        public async Task Rate30News()
        {
            var newsWithoutRating = new ConcurrentBag<NewsDto>(await _mediator
                .Send(new GetAllExistingNewsWithoutRatingQuery()));

            foreach (var news in newsWithoutRating)
            {
                var rating = await RateNews(news.Id);
                if (rating != null)
                {
                    news.Rating = rating.Value;
                    news.Status = NewsStatus.RatingCompleted;
                }
            }

 

            await UpdateRange(newsWithoutRating
                .Where(n => n.Status == NewsStatus.RatingCompleted)
                .ToList());
        }

        private async Task<double?> RateNews(Guid id)
        {
            var pureNewsText = await GetPureNewsText(id);
            
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage(HttpMethod.Post,
                "http://api.ispras.ru/texterra/v1/nlp?targetType=lemma&apikey=ae1707f570616cfd09cb49a7e1ecd91cd495aad9")
            {
                Content = new StringContent("[{\"text\":\"" + pureNewsText + "\"}]",
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await httpClient.SendAsync(request);

            var responseString = await response.Content.ReadAsStringAsync();

            if (responseString == "") return null;

            string afinn =(await System.IO.File.ReadAllTextAsync(@"AFINN-ru.json", Encoding.UTF8))
                .Replace("\n", " ");
            var values = JsonConvert.DeserializeObject<IDictionary<string, int?>>(afinn);

            dynamic stuff = JsonConvert.DeserializeObject(responseString);

            var val = stuff?.annotations.lemma;

            if (val == null || values == null) return null;

            var sum = 0;
            var count = 0;
            foreach (var item in val)
            {
                if (item.value.Value == "" || !values.ContainsKey(item.value.Value)) continue;
                count++;
                sum += values[item.value.Value];
            }

            var rating = (double)sum / count;

            return rating;
        }

        private async Task<string> GetPureNewsText(Guid id)
        {
            return await _mediator.Send(new GetPureNewsTextByIdQuery { Id = id });
        }

        public async Task<int> Add(NewsDto news)
        {
            var command = _mapper.Map<AddNewsCommand>(news);
            return await _mediator.Send(command);
        }

        public async Task<int> AddRange(IEnumerable<NewsDto> news)
        {
            return await _mediator.Send(new AddRangeNewsCommand { News = news });
        }

        public async Task<int> Update(NewsDto news)
        {
            var command = _mapper.Map<UpdateNewsCommand>(news);
            return await _mediator.Send(command);
        }

        public async Task<int> UpdateRange(IEnumerable<NewsDto> news)
        {
            return await _mediator.Send(new UpdateRangeNewsCommand { News = news });
        }

        public async Task<int> Delete(Guid id)
        {
            var command = new DeleteNewsCommand { Id = id };
            return await _mediator.Send(command);
        }

        public async Task<NewsDto> GetNewsById(Guid id)
        {
            var query = new GetNewsByIdQuery { Id = id };
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<NewsDto>> GetAllNews()
        {
            var query = new GetAllNewsQuery();
            return await _mediator.Send(query);
        }

        private IEnumerable<NewsDto> GetNewsInfoFromRssSource(RssSourceDto rssSource,
            IEnumerable<string> currentNewsUrls)
        {
            var news = new List<NewsDto>();
            var parser = _parsers.Single(p => p.Name.Equals(rssSource.Name));

            using var reader = XmlReader.Create(rssSource.Url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            if (!feed.Items.Any()) return null;
            var newFeeds = feed.Items
                .Where(i => !currentNewsUrls.Contains(parser.GetUrl(i)))
                .ToList();

            if (newFeeds.Count == 0) return null;

            foreach (var syndicationItem in newFeeds)
            {
                try
                {
                    news.Add(new NewsDto
                    {
                        Id = Guid.NewGuid(),
                        RssSourceId = rssSource.Id,
                        Author = parser.GetAuthor(syndicationItem),
                        Category = parser.GetCategory(syndicationItem),
                        Url = parser.GetUrl(syndicationItem),
                        ImageUrl = parser.GetImageUrl(syndicationItem),
                        ShortNewsFromRssSource =
                            Regex.Replace(syndicationItem.Summary.Text.Trim(), @"<.*?>", ""),
                        Title = syndicationItem.Title.Text,
                        PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime(),
                        Status = NewsStatus.RssCompleted
                    });
                }
                catch (Exception)
                {
                    Log.Error($"News information cant received from rss source {rssSource.Name}");
                }
            }

            return news;
            
        }

        public static string GetPureShortNewsFromRssSource(string shortNews)
        {

            var match = Regex.Match(shortNews, @"(?:</a></p><p>)(.*?)(?:</p>)");

            var str = match.Groups[1].Value;
 
            return Regex.Replace(str, @"<.*?>", "");
        }

        public static string GetNewsImageUrlFromRssSource(string shortNews)
        {
            var match = Regex.Match(shortNews, "(?:<img src=\")(.*?)(?:\")");
            return match.Groups[1].Value;
        }

        public async Task<NewsWithRssNameDto> GetNewsWithRssSourceNameById(Guid id)
        {
            return await _mediator.Send(new GetNewsWithRssSourceNameByIdQuery() {Id = id});
        }
    }
}
