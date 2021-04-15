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

namespace GoodNewsAggregator.Controllers
{
    public class NewsController : Controller
    {
        //private readonly GoodNewsAggregatorContext _context;

        private readonly INewsService _newsService;
        private readonly IRssSourseService _rssSourseService;
        private readonly IWebPageParser _onlinerParser;
        private readonly IMapper _mapper;

        public NewsController(IRssSourseService rssSourseService,
            INewsService newsService,
            IWebPageParser onlinerParser,
            IMapper mapper)
        {
            _rssSourseService = rssSourseService;
            _newsService = newsService;
            _onlinerParser = onlinerParser;
            _mapper = mapper;
        }


        // GET: News
        public async Task<IActionResult> Index(Guid?[] sourseIds)
        {
            IEnumerable<NewsDto> news = new List<NewsDto>();

            if (sourseIds.Length > 0)
            {
                foreach (var sourseId in sourseIds)
                {
                    var sourseNews = (await _newsService.GetNewsBySourseId(sourseId))
                        .ToList();
                    news = news.Concat(sourseNews);
                }
            }
            else
            {
                news = (await _newsService.GetNewsBySourseId(null)).ToList();
            }

            var newsList = news.Select(n => new NewsList()
            {
                Id = n.Id,
                Title = n.Title,
                Url = n.Url,
                ShortNewsFromRssSourse = n.ShortNewsFromRssSourse,
                ImageUrl = n.ImageUrl,
                PublicationDate = n.PublicationDate
            }).ToList();

            //_mapper.Map<NewsList>(news)).ToList();

            var rssSourses = (await _rssSourseService.GetAllRssSourses()).ToList();

            var newsListWithRss = new NewsListWithRss()
            {
                NewsLists = newsList,
                RssSourses = rssSourses
            };

            return View(newsListWithRss);
            
            
            
            //new NewsList()
            
            
            //{
            //    Id = n.Id,
            //    Title = n.Title,
            //    Url = n.Url,
            //    ShortNewsFromRssSourse = n.ShortNewsFromRssSourse,
            //    ImageUrl = n.ImageUrl,
            //    PublicationDate = n.PublicationDate
            //}).ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Guid[] rssIds)
        {
            //Guid[] sourseIds = Request.Headers.ContainsKey("rssIds").ToString();

            IEnumerable<NewsDto> news = new List<NewsDto>();


            if (rssIds.Length > 0)
            {
                foreach (var sourseId in rssIds)
                {
                    var sourseNews = (await _newsService.GetNewsBySourseId(sourseId))
                        .ToList();
                    news = news.Concat(sourseNews);
                }
            }
            else
            {
                news = (await _newsService.GetNewsBySourseId(null)).ToList();
            }

            var newsList = news.Select(n => new NewsList()
            {
                Id = n.Id,
                Title = n.Title,
                Url = n.Url,
                ShortNewsFromRssSourse = n.ShortNewsFromRssSourse,
                ImageUrl = n.ImageUrl,
                PublicationDate = n.PublicationDate
            }).ToList();

            //_mapper.Map<NewsList>(news)).ToList();

            var rssSourses = (await _rssSourseService.GetAllRssSourses()).ToList();

            var newsListWithRss = new NewsListWithRss()
            {
                NewsLists = newsList,
                RssSourses = rssSourses
            };



            return PartialView(newsListWithRss);
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
            //    RssSourseId = news.RssSourseId
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
            //    RssSourseId = news.RssSourseId
            //};

            return View(oneNews);
        }

        //// GET: News/Create
        //public IActionResult Create()
        //{
        //    ViewData["RssSourseId"] = new SelectList(_context.RssSourses, "Id", "Id");
        //    return View();
        //}

        //// POST: News/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Title,Url,Body,ShortNewsFromRssSourse,RssSourseId")] News news)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        news.Id = Guid.NewGuid();
        //        _context.Add(news);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RssSourseId"] = new SelectList(_context.RssSourses, "Id", "Id", news.RssSourseId);
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
        //    ViewData["RssSourseId"] = new SelectList(_context.RssSourses, "Id", "Id", news.RssSourseId);
        //    return View(news);
        //}

        //// POST: News/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Url,Body,ShortNewsFromRssSourse,RssSourseId")] News news)
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
        //    ViewData["RssSourseId"] = new SelectList(_context.RssSourses, "Id", "Id", news.RssSourseId);
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
        //        .Include(n => n.RssSourse)
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
        public async Task<IActionResult> Aggregate(CreateNewsViewModel sourse)
        {
            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var rssSourses = await _rssSourseService
                    .GetAllRssSourses();
                var newInfos = new List<NewsDto>();

                foreach (var rssSourse in rssSourses)
                {
                    var newsList = await _newsService
                        .GetNewsInfoFromRssSourse(rssSourse);
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

                await _newsService.AddRange(newInfos);
                stopwatch.Stop();
                Log.Information($"Aggregation was executed in {stopwatch.ElapsedMilliseconds}");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Aggregation error {e.Message}");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
