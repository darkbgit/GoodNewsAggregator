using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Runtime.InteropServices;
using System.Text;

using System.Text.RegularExpressions;
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

        public async Task<IEnumerable<NewsDto>> GetNewsBySourceId(Guid? id)
        {
            throw new NotImplementedException();
            //if (!id.HasValue)
            //{
            //    Log.Warning(nameof(id) + " in " + ToString() + " was null");
            //}

            //var news = id.HasValue
            //    ? await _unitOfWork.News.FindBy(n
            //            => n.RssSourceId.Equals(id.GetValueOrDefault()))
            //        .ToListAsync()
            //    : await _unitOfWork.News.FindBy(n => n.Id != null)
            //        .ToListAsync();

            //return news.Select(n => _mapper.Map<NewsDto>(n)).ToList();

        }

        public async Task<Tuple<IEnumerable<NewsDto>, int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder)
        {
            throw new NotImplementedException();
            ////IQueryable<News> news = Enumerable.Empty<News>().AsQueryable();
            //IEnumerable<News> news;
            //IEnumerable<News> newsEnumerable;
            //IEnumerable<NewsDto> newsDtoList;
            //int count;
            ////if (!rssIds.Any())
            ////{
            ////    news = _unitOfWork.News.FindBy(n =>
            ////        !string.IsNullOrEmpty(n.Url));
            ////    count = news.Count();
            ////    newsDtoList = news.OrderByDescending(n => n.PublicationDate)
            ////        .Skip((pageNumber - 1) * newsPerPage)
            ////        .Take(newsPerPage)
            ////        .Select(n => _mapper.Map<NewsDto>(n))
            ////        .ToList();
            ////}
            ////else
            //{
            //    news = _unitOfWork.News.FindBy(n =>
            //        rssIds.Contains(n.RssSourceId));
            //    count = news.Count();

            //    switch (sortOrder)
            //    {
            //        case "Date":
            //            news = news.OrderBy(n => n.PublicationDate);
            //            break;
            //        //case "Rating":
            //        //    news = news.OrderBy(n => n.PublicationDate).ToList();
            //        //    break;
            //        //case "rating_desc":
            //        //    news = news.OrderBy(n => n.PublicationDate).ToList();
            //        //    break;
            //        default:
            //            news = news.OrderByDescending(n => n.PublicationDate);
            //            break;
            //    }

            //    newsDtoList = news.Skip((pageNumber - 1) * newsPerPage)
            //        .Take(newsPerPage)
            //        .Select(n => _mapper.Map<NewsDto>(n))
            //        .ToList();
            //}
            //return new Tuple<IEnumerable<NewsDto>, int>(newsDtoList, count);
        }

        public async Task Aggregate()
        {
            var news = new ConcurrentBag<NewsDto>();
            var rssSources = await _mediator.Send(new GetAllRssSourceQuery());
            var currentNewsUrls = await _mediator.Send(new GetAllExistingNewsUrlsQuery());

            Parallel.ForEach(rssSources, (rssSource) =>
            {
                var parser = _parsers.Single(p => p.Name.Equals(rssSource.Name));

                using var reader = XmlReader.Create(rssSource.Url);
                var feed = SyndicationFeed.Load(reader);
                reader.Close();

                if (feed.Items.Any())
                {
                    foreach (var syndicationItem in feed.Items
                        .Where(i => !currentNewsUrls.Contains(parser.GetUrl(i))))
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
                            Log.Error("News information cant received from rss source");
                        }
                    }
                }

            });

            Parallel.ForEach(news, async (n) =>
            {
                try
                {
                    n.Body = await GetNewsBodyInfoFromSite(n.Url);
                    n.Status = NewsStatus.BodyCompleted;
                }
                catch (Exception)
                {
                    Log.Error($"News {n.Url} cant parse body");
                }
                
            });

            Parallel.ForEach(news, async (n) =>
            {
                try
                {
                    n.Body = await GetNewsBodyInfoFromSite(n.Url);
                    n.Status = NewsStatus.BodyCompleted;
                }
                catch (Exception)
                {
                    Log.Error($"News {n.Url} cant take rating");
                }

            });

            await AddRange(news);

            var newsWithoutBody = new ConcurrentBag<NewsWithRssNameDto>(await _mediator
                .Send(new GetAllExistingNewsWithoutBodyQuery()));

            Parallel.ForEach(newsWithoutBody, async (n) =>
            {
                var parser = _parsers.Single(p => p.Name.Equals(n.RssSourceName));
                n.Body = await parser.GetBody(n.Url);
                n.Status = NewsStatus.BodyCompleted;
            });

            var newsWithoutRating = new ConcurrentBag<NewsDto>(await _mediator
                .Send(new GetAllExistingNewsWithoutRatingQuery()));

            Parallel.ForEach(newsWithoutRating, async n =>
            {
                n.Rating = await RateNews(n.Id);
                n.Status = NewsStatus.RatingCompleted;
            });

            var updatedNews = newsWithoutBody.Select(
                n => new NewsDto()
                {
                    Id = n.Id,
                    Body = n.Body
                })
                .Concat(newsWithoutRating
                    .Select(n => new NewsDto()
                    {
                        Id = n.Id,
                        Rating = n.Rating
                    }));

            await UpdateRange(news);
        }

        public async Task<double> RateNews(Guid id)
        {
            var pureNewsText = await _mediator.Send(new GetPureNewsTextByIdQuery {Id = id});
            
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

            if (responseString == null) return 0;

            string afinn =(await System.IO.File.ReadAllTextAsync(@"AFINN-ru.json", Encoding.UTF8))
                .Replace("\n", " ");
            var values = JsonConvert.DeserializeObject<IDictionary<string, int?>>(afinn);

            dynamic stuff = JsonConvert.DeserializeObject(responseString);

            var val = stuff?.annotations.lemma;

            if (val == null || values == null) return default;

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

        public async Task<string> GetPureNewsText(Guid id)
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

        public async Task<IEnumerable<NewsDto>> GetNewsInfoFromRssSource(RssSourceDto rssSource)
        {
            var parser = _parsers.Single(p => p.Name.Equals(rssSource.Name));

            var news = new ConcurrentBag<NewsDto>();

            using var reader = XmlReader.Create(rssSource.Url);
            var feed = SyndicationFeed.Load(reader);
            reader.Close();

            if (!feed.Items.Any()) return news;
                
            var currentNewsUrls = await _mediator.Send(new GetAllExistingNewsUrlsQuery());

            foreach (var syndicationItem in feed.Items
                .Where(i => !currentNewsUrls.Contains(parser.GetUrl(i))))
            {
                try
                {
                    news.Add( new NewsDto
                    {
                        Id = Guid.NewGuid(),
                        RssSourceId = rssSource.Id,
                        Author = parser.GetAuthor(syndicationItem),
                        Category = parser.GetCategory(syndicationItem),
                        Url = parser.GetUrl(syndicationItem),
                        ImageUrl = parser.GetImageUrl(syndicationItem),
                        ShortNewsFromRssSource = Regex.Replace(syndicationItem.Summary.Text.Trim(), @"<.*?>", ""),
                        Title = syndicationItem.Title.Text,
                        PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime(),
                        Status = NewsStatus.RssCompleted
                    });
                }
                catch (Exception)
                {
                    Log.Error("News information cant received from rss source");
                }
                        
            }

            return news;
        }

        private async Task<string> GetNewsBodyInfoFromSite(string url)
        {

            return null;
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
            throw new NotImplementedException();
            //var result = await _unitOfWork.News
            //    .FindBy(n => n.Id.Equals(id),
            //        n => n.RssSource)
            //    .FirstOrDefaultAsync();
            //return _mapper.Map<NewsWithRssNameDto>(result);
        }
    }
}
