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
            var news = new List<NewsDto>();
            using (var reader = XmlReader.Create(rssSource.Url))
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
                            var newsDto = new NewsDto
                            {
                                Id = Guid.NewGuid(),
                                RssSourceId = rssSource.Id
                            };
                            switch (rssSource.Name)
                            {
                                case "TJournal":
                                    newsDto.Author = syndicationItem.Authors?[0]?.Email;
                                    newsDto.Url =
                                        syndicationItem.Links.FirstOrDefault(sl => sl.RelationshipType.Equals("alternate"))?.Uri.AbsoluteUri;
                                    newsDto.ImageUrl =
                                        syndicationItem.Links.FirstOrDefault(sl =>
                                            sl.RelationshipType.Equals("enclosure"))?.Uri.AbsoluteUri;
                                    newsDto.ShortNewsFromRssSource =  syndicationItem.Summary.Text.Trim();
                                    break;
                                case "S13":
                                    continue;
                                    break;
                                case "DTF":
                                    continue;
                                    break;
                                case "Tut.by":
                                    continue;
                                    break;
                                case "Onliner":
                                    continue;
                                    break;
                                default:
                                    Log.Error("RSS source name is undefined");
                                    break;
                            }




                            //newsDto.Url = syndicationItem.Id;
                            newsDto.Title = syndicationItem.Title.Text;
                                //ShortNewsFromRssSource = syndicationItem.Summary.Text
                                newsDto.ShortNewsFromRssSource =
                                    GetPureShortNewsFromRssSource(syndicationItem.Summary.Text);
                                //newsDto.ImageUrl = GetNewsImageUrlFromRssSource(syndicationItem.Summary.Text);
                                newsDto.PublicationDate = syndicationItem.PublishDate.DateTime.ToUniversalTime();

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
