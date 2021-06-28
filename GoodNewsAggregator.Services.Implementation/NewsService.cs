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
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using Newtonsoft.Json;

namespace GoodNewsAggregator.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IWebPageParser> _parsers;
        private readonly IRssSourceService _rssSourceService;

        public NewsService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            IEnumerable<IWebPageParser> parsers,
            IRssSourceService rssSourceService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _parsers = parsers;
            _rssSourceService = rssSourceService;
        }

        public async Task<IEnumerable<NewsDto>> GetNewsByRssSourceIds(Guid[] ids)
        {
            var news = ids.Any()
                ? await _unitOfWork.News.FindBy(n
                        => ids.Contains(n.RssSourceId))
                    .ToListAsync()
                : await _unitOfWork.News.FindBy(n => n.Id != null)
                    .ToListAsync();

            return news.Select(n => _mapper.Map<NewsDto>(n)).ToList();
        }

        public async Task<Tuple<IEnumerable<NewsDto>, int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder,
            double? minimalRating)
        {

            var news = await _unitOfWork.News.FindBy(n =>
                    rssIds.Contains(n.RssSourceId))
                .Where(n => n.Status >= NewsStatus.BodyCompleted)
                .ToListAsync();

            if (minimalRating != null)
            {
                news = news.Where(n => n.Rating >= minimalRating.Value).ToList();
            }
            var count = news.Count;

            switch (sortOrder)
            {
                case "Date":
                    news = news.OrderBy(n => n.PublicationDate).ToList();
                    break;
                case "Rating":
                    news = news.OrderBy(n => n.Rating).ToList();
                    break;
                case "rating_desc":
                    news = news.OrderByDescending(n => n.Rating).ToList();
                    break;
                default:
                    news = news.OrderByDescending(n => n.PublicationDate).ToList();
                    break;
            }

            var newsDtoList = news.Skip((pageNumber - 1) * newsPerPage)
                .Take(newsPerPage)
                .ToList();
            var result = newsDtoList
                .Select(n => _mapper.Map<NewsDto>(n))
                .ToList();

            return new Tuple<IEnumerable<NewsDto>, int>(result, count);
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

            string afinn = (await System.IO.File.ReadAllTextAsync(@"AFINN-ru.json", Encoding.UTF8))
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

        public async Task Rate30News()
        {
            var newsWithoutRating = new ConcurrentBag<NewsDto>(await _unitOfWork.News
                .FindBy(n => n.Status == NewsStatus.BodyCompleted)
                .Select(n => _mapper.Map<NewsDto>(n))
                .Take(30)
                .ToListAsync());

            Parallel.ForEach(newsWithoutRating, async (n) =>
            {
                var rating = await RateNews(n.Id);
                if (rating != null)
                {
                    n.Rating = rating.Value;
                    n.Status = NewsStatus.RatingCompleted;
                }
            });

            await UpdateRange(newsWithoutRating
                .Where(n => n.Status == NewsStatus.RatingCompleted)
                .ToList());
        }

        public async Task<string> GetPureNewsText(Guid id)
        {
            var body = (await _unitOfWork.News
                    .Get(id))
                    .Body;

            string pattern = @"(?:<).*?(?:>)";
            string target = " ";
            var rgx = new Regex(pattern);
            var result = rgx.Replace(body, target);
            return result;
        }

        public async Task<int> Add(NewsDto news)
        {
            var entity = _mapper.Map<News>(news);

            await _unitOfWork.News.Add(entity);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> AddRange(IEnumerable<NewsDto> news)
        {
            var entities = news.Select(ent => _mapper.Map<News>(ent)).ToList();

            await _unitOfWork.News.AddRange(entities);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> Update(NewsDto news)
        {
            var entity = _mapper.Map<News>(news);
            _unitOfWork.News.Update(entity);
            var result = await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<int> UpdateRange(IEnumerable<NewsDto> news)
        {
            var entities = news.Select(ent => _mapper.Map<News>(ent)).ToList();
            _unitOfWork.News.UpdateRange(entities);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> Delete(Guid id)
        {
            _unitOfWork.News.Remove(id);
            var result = await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<NewsDto> GetNewsById(Guid id)
        {
            var entity = await _unitOfWork.News.Get(id);
            return _mapper.Map<NewsDto>(entity);
        }

        public async Task<IEnumerable<NewsDto>> GetAllNews()
        {
            var news = await _unitOfWork.News.GetAll().ToListAsync();
            return news.Select(n => _mapper.Map<NewsDto>(news)).ToList();
        }

        public async Task Aggregate()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var rssSources = await _rssSourceService
                .GetAllRssSources();
            var newInfos = new List<NewsDto>();

            foreach (var rssSource in rssSources)
            {
                try
                {
                    var newsList = await GetNewsInfoFromRssSource(rssSource);
                    newInfos.AddRange(newsList);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Aggregation error {e.Message}");
                }
            }

            try
            {
                await AddRange(newInfos);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Aggregation error {e.Message}");
            }
            stopwatch.Stop();
            Log.Information($"Aggregation was executed in {stopwatch.ElapsedMilliseconds}");
        }

        public async Task GetBodies()
        {
            var newsWithoutBody = new ConcurrentBag<NewsWithRssNameDto>(await _unitOfWork
                .News
                .FindBy(n => n.Status == NewsStatus.RssCompleted)
                .Include(n => n.RssSource)
                .Select(n => new NewsWithRssNameDto()
                {
                    Id = n.Id,
                    RssSourceName = n.RssSource.Name
                })
                .ToListAsync());

            Parallel.ForEach(newsWithoutBody, async (n) =>
            {
                try
                {
                    var parser = _parsers.Single(p => p.Name.Equals(n.RssSourceName));
                    var body = parser.GetBody(n.Url);
                    if (body == null) return;
                    n.Body = body;
                    n.Status = NewsStatus.BodyCompleted;
                }
                catch
                {
                    Log.Error($"Cant take body from news url {n.Url}");
                }

            });

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

        public async Task<IEnumerable<NewsDto>> GetNewsInfoFromRssSource(RssSourceDto rssSource)
        {
            var parser = _parsers.Single(p => p.Name.Equals(rssSource.Name));

            //var news = await parser?.ParseRss(rssSource);

            var news = new List<NewsDto>();
            using (var reader = XmlReader.Create(rssSource.Url))
            {
                var feed = SyndicationFeed.Load(reader);
                reader.Close();
                if (feed.Items.Any())
                {
                    news.AddRange(feed.Items.Select(syndicationItem => new NewsDto
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
                    }));
                }
            }

            var currentNewsUrls = await _unitOfWork.News
                .GetAll()
                .Select(n => n.Url)
                .ToListAsync();

            var newsList = news.ToList();

            var newNews = newsList.Where(n => !currentNewsUrls.Any(url => url.Equals(n.Url)));
            foreach (var item in newNews)
            {
                item.Body = "";// await parser.GetBody(item.Url) ?? "";
            };


            return newNews;
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
            var result = await _unitOfWork.News
                .FindBy(n => n.Id.Equals(id),
                    n => n.RssSource)
                .FirstOrDefaultAsync();
            return _mapper.Map<NewsWithRssNameDto>(result);
        }
    }
}
