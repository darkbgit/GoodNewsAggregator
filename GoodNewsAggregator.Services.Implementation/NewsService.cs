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

        public NewsService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NewsDto>> GetNewsBySourseId(Guid? id)
        {
            if (!id.HasValue)
            {
                Log.Warning(nameof(id) + " in " + ToString() + " was null");
            }

            var news = id.HasValue
                ? await _unitOfWork.News.FindBy(n
                        => n.RssSourseId.Equals(id.GetValueOrDefault()))
                    .ToListAsync()
                : await _unitOfWork.News.FindBy(n => n.Id != null)
                    .ToListAsync();

            return news.Select(n => _mapper.Map<NewsDto>(n)).ToList();
 
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
        //        var rssSourses =  _unitOfWork.RssSourses.GetAll().ToList();
        //        var newInfos = new List<NewsDto>();

        //        foreach (var rssSourse in rssSourses)
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

        public async Task<IEnumerable<NewsDto>> GetNewsInfoFromRssSourse(RssSourseDto rssSourse)
        {
            var news = new List<NewsDto>();
            using (var reader = XmlReader.Create(rssSourse.Url))
            {
                var feed = SyndicationFeed.Load(reader);
                reader.Close();
                if (feed.Items.Any())
                {
                    var currentNewsUrls = await _unitOfWork.News
                        .Get()
                        .Select(n => n.Url)
                        .ToListAsync();
                    foreach (var syndicationItem in feed.Items)
                    {
                        if (!currentNewsUrls.Any(url => url.Equals(syndicationItem.Id)))
                        {
                            var newsDto = new NewsDto()
                            {
                                Id = Guid.NewGuid(),
                                RssSourseId = rssSourse.Id,
                                Url = syndicationItem.Id,
                                Title = syndicationItem.Title.Text,
                                //ShortNewsFromRssSourse = syndicationItem.Summary.Text
                                ShortNewsFromRssSourse = GetPureShortNewsFromRssSource(syndicationItem.Summary.Text),
                                ImageUrl = GetNewsImageUrlFromRssSource(syndicationItem.Summary.Text),
                                PublicationDate = syndicationItem.PublishDate.DateTime.ToString()
                            };
                            news.Add(newsDto);
                        }
                        
                    }
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
    }
}
