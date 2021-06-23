using System;
using System.Collections.Concurrent;
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
using GoodNewsAggregator.DAL.CQRS.Commands.NewsC;
using GoodNewsAggregator.DAL.CQRS.Queries.NewsQ;
using GoodNewsAggregator.DAL.CQRS.Queries.RssSourceQ;
using MediatR;

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
            var rssSources = await _mediator.Send(new GetAllRssSourceQuery());

        }

        public async Task<int> Add(NewsDto news)
        {
            var command = _mapper.Map<AddNewsCommand>(news);
            return await _mediator.Send(command);
        }

        public async Task<int> AddRange(IEnumerable<NewsDto> news)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update(NewsDto news)
        {
            var command = _mapper.Map<UpdateNewsCommand>(news);
            return await _mediator.Send(command);
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

            using (var reader = XmlReader.Create(rssSource.Url))
            {
                var feed = SyndicationFeed.Load(reader);
                reader.Close();
                if (feed.Items.Any())
                {
                    var currentNewsUrls = await _mediator.Send(new GetAllExistingNewsUrlsQuery());

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
                            PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime()
                        };
                        if (!currentNewsUrls.Contains(newsDto.Url))
                        {
                            news.Add(newsDto);
                        }
                    }
                }
            }

            
         

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
            throw new NotImplementedException();
            //var result = await _unitOfWork.News
            //    .FindBy(n => n.Id.Equals(id),
            //        n => n.RssSource)
            //    .FirstOrDefaultAsync();
            //return _mapper.Map<NewsWithRssNameDto>(result);
        }
    }
}
