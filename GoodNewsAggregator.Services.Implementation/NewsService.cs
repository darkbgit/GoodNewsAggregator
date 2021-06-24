using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Runtime.InteropServices;
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

namespace GoodNewsAggregator.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IWebPageParser> _parsers;

        public NewsService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapper mapper,
            IEnumerable<IWebPageParser> parsers)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
            _parsers = parsers;
        }

        public async Task<IEnumerable<NewsDto>> GetNewsBySourceId(Guid? id)
        {
            if (!id.HasValue)
            {
                Log.Warning(nameof(id) + " in " + ToString() + " was null");
            }

            var news = id.HasValue
                ? await _unitOfWork.News.FindBy(n
                        => n.RssSourceId.Equals(id.GetValueOrDefault()))
                    .ToListAsync()
                : await _unitOfWork.News.FindBy(n => n.Id != null)
                    .ToListAsync();

            return news.Select(n => _mapper.Map<NewsDto>(n)).ToList();
        }

        public async Task<Tuple<IEnumerable<NewsDto>, int>> GetNewsPerPage(Guid[] rssIds,
            int pageNumber,
            int newsPerPage,
            string sortOrder)
        {
            //IQueryable<News> news = Enumerable.Empty<News>().AsQueryable();
            IEnumerable<News> news;
            IEnumerable<News> newsEnumerable;
            IEnumerable<NewsDto> newsDtoList;
            int count;
            //if (!rssIds.Any())
            //{
            //    news = _unitOfWork.News.FindBy(n =>
            //        !string.IsNullOrEmpty(n.Url));
            //    count = news.Count();
            //    newsDtoList = news.OrderByDescending(n => n.PublicationDate)
            //        .Skip((pageNumber - 1) * newsPerPage)
            //        .Take(newsPerPage)
            //        .Select(n => _mapper.Map<NewsDto>(n))
            //        .ToList();
            //}
            //else
            {
                news = _unitOfWork.News.FindBy(n =>
                    rssIds.Contains(n.RssSourceId));
                count = news.Count();

                switch (sortOrder)
                {
                    case "Date":
                        news = news.OrderBy(n => n.PublicationDate);
                        break;
                    //case "Rating":
                    //    news = news.OrderBy(n => n.PublicationDate).ToList();
                    //    break;
                    //case "rating_desc":
                    //    news = news.OrderBy(n => n.PublicationDate).ToList();
                    //    break;
                    default:
                        news = news.OrderByDescending(n => n.PublicationDate);
                        break;
                }

                newsDtoList = news.Skip((pageNumber - 1) * newsPerPage)
                    .Take(newsPerPage)
                    .Select(n => _mapper.Map<NewsDto>(n))
                    .ToList();
            }
            return new Tuple<IEnumerable<NewsDto>, int>(newsDtoList, count);
        }

        public Task<double> RateNews(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPureNewsText(Guid id)
        {
            throw new NotImplementedException();
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
            await _unitOfWork.News.Update(entity);
            var result = await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<int> Delete(Guid id)
        {
            await _unitOfWork.News.Remove(id);
            var result = await _unitOfWork.SaveChangesAsync();
            return result;
        }

        public async Task<NewsDto> GetNewsById(Guid id)
        {
            var entity = await _unitOfWork.News.Get(id);
            return _mapper.Map<NewsDto>(entity);
        }

        public Task<IEnumerable<NewsDto>> GetAllNews()
        {
            throw new NotImplementedException();
        }

        public Task Aggregate()
        {
            throw new NotImplementedException();
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
