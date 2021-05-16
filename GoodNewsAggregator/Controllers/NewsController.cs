using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.News;
using System.Diagnostics;
using GoodNewsAggregator.Core.DTOs;
using Serilog;
using AutoMapper;
using GoodNewsAggregator.Models;
using GoodNewsAggregator.Models.ViewModels;
using GoodNewsAggregator.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace GoodNewsAggregator.Controllers
{
    [Authorize]
    public class NewsController : Controller
    {
        //private readonly GoodNewsAggregatorContext _context;

        private readonly INewsService _newsService;
        private readonly IRssSourceService _rssSourceService;
        private readonly IWebPageParser _onlinerParser;
        private readonly IMapper _mapper;

        public NewsController(IRssSourceService rssSourceService,
            INewsService newsService,
            IWebPageParser onlinerParser,
            IMapper mapper)
        {
            _rssSourceService = rssSourceService;
            _newsService = newsService;
            _onlinerParser = onlinerParser;
            _mapper = mapper;
        }


        // GET: News
        [HttpGet]
        public async Task<IActionResult> Index(Guid?[] sourceIds, int page = 1)
        {
            IEnumerable<NewsDto> news = new List<NewsDto>();

            if (sourceIds.Length > 0)
            {
                foreach (var sourceId in sourceIds)
                {
                    var sourceNews = (await _newsService.GetNewsBySourceId(sourceId))
                        .OrderByDescending(n => n.PublicationDate)
                        .ToList();
                    news = news.Concat(sourceNews);
                }
            }
            else
            {
                news = (await _newsService.GetNewsBySourceId(null))
                    .OrderByDescending(n=>n.PublicationDate)
                    .ToList();
            }


            var newsPerPage = news.Skip((page - 1) * Constants.NEWS_PER_PAGE)
                .Take(Constants.NEWS_PER_PAGE);

            var newsList = newsPerPage.Select(n => new NewsList()
            {
                Id = n.Id,
                Title = n.Title,
                Url = n.Url,
                ShortNewsFromRssSource = n.ShortNewsFromRssSource,
                ImageUrl = n.ImageUrl,
                PublicationDate = n.PublicationDate,
                Author = n.Author,
                Category = n.Category
            }).ToList();

            //_mapper.Map<NewsList>(news)).ToList();

            var rssSources = (await _rssSourceService.GetAllRssSources()).ToList();

            var rssList = rssSources.Select(r => new RssList()
            {
                Id = r.Id,
                Name = r.Name,
                Url = r.Url,
                Checked = !sourceIds.Any() || sourceIds.Contains(r.Id)
            });

            var pageInfo = new PageInfo(page, news.Count());


            var newsListWithRss = new NewsListWithRssWithPagination()
            {
                NewsLists = newsList,
                RssList = rssList,
                Pagination = pageInfo
            };

            //if (!HttpContext.User.Identity.IsAuthenticated)
            //{
            //    return Content(
            //        "<script language='javascript' type='text/javascript'>document.querySelector('button[data-toggle='ajax-modal']').click();</script>");
            //    //HttpContext.Response.Headers.Add("REQUIRES_AUTH", "1");
            //}

            return View(newsListWithRss);
            
            
            
            //new NewsList()
            
            
            //{
            //    Id = n.Id,
            //    Title = n.Title,
            //    Url = n.Url,
            //    ShortNewsFromRssSource = n.ShortNewsFromRssSource,
            //    ImageUrl = n.ImageUrl,
            //    PublicationDate = n.PublicationDate
            //}).ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Guid[] rssIds, int page = 1)
        {
            //Guid[] sourceIds = Request.Headers.ContainsKey("rssIds").ToString();
            //Request.Headers.TryGetValue("rssIds", sourceIds).ToList();

            IEnumerable<NewsDto> news = new List<NewsDto>();


            foreach (var sourceId in rssIds)
            {
                var sourceNews = (await _newsService.GetNewsBySourceId(sourceId))
                    .ToList();
                news = news.Concat(sourceNews);
            }

            var newsDtoList = news
                .OrderByDescending(n => n.PublicationDate)
                .ToList();


            var newsPerPageCount = 24;

            var newsPerPage = newsDtoList.Skip((page - 1) * newsPerPageCount).Take(newsPerPageCount);

            var newsList = newsPerPage.Select(n => new NewsList()
            {
                Id = n.Id,
                Title = n.Title,
                Url = n.Url,
                ShortNewsFromRssSource = n.ShortNewsFromRssSource,
                ImageUrl = n.ImageUrl,
                PublicationDate = n.PublicationDate,
                Author = n.Author,
                Category = n.Category
            }).ToList();


            var pageInfo = new PageInfo(page, newsDtoList.Count);

            //_mapper.Map<NewsList>(news)).ToList();

            var newsListsWithPagination = new NewsListWithPagination()
            {
                NewsLists = newsList,
                Pagination = pageInfo
            };



            return PartialView("_NewsListsWithPagination", newsListsWithPagination);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _newsService
                .GetNewsById(id.Value);
                
            if (news == null)
            {
                return NotFound();
            }

            var oneNews = _mapper.Map<OneNews>(news);

            //    new OneNews()
            //{
            //    Id = news.Id,
            //    Title = news.Title,
            //    Url = news.Url,
            //    Body = news.Body,
            //    ImageUrl = news.ImageUrl,
            //    PublicationDate = news.PublicationDate,
            //    RssSourceId = news.RssSourceId
            //};

            

            return View(oneNews);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Read(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _newsService
                .GetNewsById(id.Value);

            if (news == null)
            {
                return NotFound();
            }

            var oneNews = _mapper.Map<OneNews>(news);
            //new OneNews()
            //{
            //    Id = news.Id,
            //    Title = news.Title,
            //    Url = news.Url,
            //    Body = news.Body,
            //    ImageUrl = news.ImageUrl,
            //    PublicationDate = news.PublicationDate,
            //    RssSourceId = news.RssSourceId
            //};

            return View(oneNews);
        }

        //// GET: News/Create
        //public IActionResult Create()
        //{
        //    ViewData["RssSourceId"] = new SelectList(_context.RssSources, "Id", "Id");
        //    return View();
        //}

        //// POST: News/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Title,Url,Body,ShortNewsFromRssSource,RssSourceId")] News news)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        news.Id = Guid.NewGuid();
        //        _context.Add(news);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RssSourceId"] = new SelectList(_context.RssSources, "Id", "Id", news.RssSourceId);
        //    return View(news);
        //}

        //// GET: News/Edit/5
        //public async Task<IActionResult> Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var news = await _context.News.FindAsync(id);
        //    if (news == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["RssSourceId"] = new SelectList(_context.RssSources, "Id", "Id", news.RssSourceId);
        //    return View(news);
        //}

        //// POST: News/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Url,Body,ShortNewsFromRssSource,RssSourceId")] News news)
        //{
        //    if (id != news.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(news);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NewsExists(news.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RssSourceId"] = new SelectList(_context.RssSources, "Id", "Id", news.RssSourceId);
        //    return View(news);
        //}

        //// GET: News/Delete/5
        //public async Task<IActionResult> Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var news = await _context.News
        //        .Include(n => n.RssSource)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (news == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(news);
        //}

        //// POST: News/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(Guid id)
        //{
        //    var news = await _context.News.FindAsync(id);
        //    _context.News.Remove(news);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool NewsExists(Guid id)
        //{
        //    return _context.News.Any(e => e.Id == id);
        //}

        public IActionResult Aggregate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aggregate(CreateNewsViewModel source)
        {
            //try
            //{
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var rssSources = await _rssSourceService
                    .GetAllRssSources();
                var newInfos = new List<NewsDto>();

                foreach (var rssSource in rssSources)
                {
                try
                {
                    var newsList = await _newsService
                        .GetNewsInfoFromRssSource(rssSource);
                    //if (rssSourse.Name.Equals("Onliner"))
                    //{
                    //    foreach (var newsDto in newsList)
                    //    {
                    //        var newsBody = await _onlinerParser.Parse(newsDto.Url);
                    //        newsDto.Body = newsBody;
                    //    }
                    //}

                    newInfos.AddRange(newsList);
                }
                catch (Exception e)
                {
                    Log.Error(e, $"Aggregation error {e.Message}");
                }
            }

            try 
            { 
            await _newsService.AddRange(newInfos);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Aggregation error {e.Message}");
            }
            stopwatch.Stop();
            Log.Information($"Aggregation was executed in {stopwatch.ElapsedMilliseconds}");

            return RedirectToAction(nameof(Index));
        }
    }
}
