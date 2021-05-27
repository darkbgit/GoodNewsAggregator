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

        public async Task<Tuple<IEnumerable<NewsDto>, int>> GetNewsPerPage(Guid[]? rssIds, int pageNumber,
            int newsPerPage,
            int orderBy = 1)
        {
            //IQueryable<News> news = Enumerable.Empty<News>().AsQueryable();
            IQueryable<News> news;
            IEnumerable<NewsDto> newsPerPageList;
            int count;
            if (rssIds == null)
            {
                news = _unitOfWork.News.FindBy(n =>
                    !string.IsNullOrEmpty(n.Url));
                count = await news.CountAsync();
                newsPerPageList = await news.OrderByDescending(n => n.PublicationDate)
                    .Skip((pageNumber - 1) * newsPerPage)
                    .Take(newsPerPage)
                    .Select(n => _mapper.Map<NewsDto>(n))
                    .ToListAsync();
            }
            else
            {
                news = _unitOfWork.News.FindBy(n =>
                    rssIds.Contains(n.RssSourceId));
                count = await news.CountAsync();
                newsPerPageList = await news.OrderByDescending(n => n.PublicationDate)
                    .Skip((pageNumber - 1) * newsPerPage)
                    .Take(newsPerPage)
                    .Select(n => _mapper.Map<NewsDto>(n))
                    .ToListAsync();
            }
            return new Tuple<IEnumerable<NewsDto>, int>(newsPerPageList, count);
        }

        public Task Add(NewsDto news)
        {
            throw new NotImplementedException();
        }

        public async Task AddRange(IEnumerable<NewsDto> news)
        {
            var entities = news.Select(ent => _mapper.Map<News>(ent)).ToList();

            await _unitOfWork.News.AddRange(entities);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task<int> Edit(NewsDto news)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(NewsDto news)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<NewsDto>> AggregateNewsFromRssSourse()
        //{
        //    try
        //    {
        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();
        //        var rssSourses =  _unitOfWork.RssSources.GetAll().ToList();
        //        var newInfos = new List<NewsDto>();

        //        foreach (var rssSource in rssSourses)
        //        {
        //            var newList = await _unitOfWork.News.
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //        throw;
        //    }
        //}

        public async Task<NewsDto> GetNewsById(Guid id)
        {
            var entity = await _unitOfWork.News.GetById(id);

            return _mapper.Map<NewsDto>(entity);
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
                    foreach (var syndicationItem in feed.Items)
                    {
                        var newsDto = new NewsDto
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

                        };
                        news.Add(newsDto);
                    }
                }
            }

            var currentNewsUrls = await _unitOfWork.News
                .Get()
                .Select(n => n.Url)
                .ToListAsync();

            var newsList = news.ToList();

            var newNews = newsList.Where(n => !currentNewsUrls.Any(url => url.Equals(n.Url)));
            foreach (var item in newNews)
            {
                item.Body = await parser.GetBody(item.Url) ?? "";
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
    }
}
