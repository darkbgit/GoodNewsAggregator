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
using GoodNewsAggregator.Utilities.Enums;

namespace GoodNewsAggregator.Controllers
{
    //[Authorize]
    public class NewsController : Controller
    {
        //private readonly GoodNewsAggregatorContext _context;

        private readonly INewsService _newsService;
        private readonly IRssSourceService _rssSourceService;
        private readonly ICommentService _commentService;
        private readonly IWebPageParser _onlinerParser;
        private readonly IMapper _mapper;

        public NewsController(IRssSourceService rssSourceService,
            INewsService newsService,
            IWebPageParser onlinerParser,
            IMapper mapper, ICommentService commentService)
        {
            _rssSourceService = rssSourceService;
            _newsService = newsService;
            _onlinerParser = onlinerParser;
            _mapper = mapper;
            _commentService = commentService;
        }


        // GET: News
        [HttpGet]
        public async Task<IActionResult> Index(Guid[] rssIds, int page = 1)
        {

            var rssSources = (await _rssSourceService.GetAllRssSources()).ToList();

            if (!rssIds.Any())
            {
                rssIds = rssSources.Select(r => r.Id).ToArray();
            }

            var rssList = rssSources.Select(r => new RssList()
            {
                Id = r.Id,
                Name = r.Name,
                Url = r.Url,
                Checked = !rssIds.Any() || rssIds.Contains(r.Id)
            });

            var (newsPerPage, count) = await _newsService.GetNewsPerPage(rssIds,
                page,
                Constants.NEWS_PER_PAGE,
                "");

            var newsList = newsPerPage.Select(n => _mapper.Map<NewsList>(n)).ToList();

            var pageInfo = new PageInfo(page, count);

            var newsListWithRssWithPagination = new NewsListWithRssWithPagination()
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

            return View(newsListWithRssWithPagination);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Guid[] rssIds,
            string sortOrder,
            int page = 1)
        {
            //Guid[] sourceIds = Request.Headers.ContainsKey("rssIds").ToString();
            //Request.Headers.TryGetValue("rssIds", sourceIds).ToList();

            //IQueryable<NewsDto> news = Enumerable.Empty<NewsDto>().AsQueryable();

            //foreach (var sourceId in rssIds)
            //{
            //    var sourceNews = (await _newsService.GetNewsBySourceId(sourceId));
            //    news = news.Concat(sourceNews);
            //}

            //var count = await news.CountAsync();
            //var newsPerPage =  await news
            //    .OrderByDescending(n => n.PublicationDate)
            //    .Skip((page - 1) * Constants.NEWS_PER_PAGE)
            //    .Take(Constants.NEWS_PER_PAGE)
            //    .ToListAsync();


            //var newsPerPage = news.Skip((page - 1) * Constants.NEWS_PER_PAGE).Take(Constants.NEWS_PER_PAGE);
            var (newsPerPage, count) = await  _newsService.GetNewsPerPage(rssIds,
                page,
                Constants.NEWS_PER_PAGE,
                sortOrder);

            var newsList = newsPerPage.Select(n => _mapper.Map<NewsList>(n)).ToList();
            
            
            //    new NewsList()
            //{
            //    Id = n.Id,
            //    Title = n.Title,
            //    Url = n.Url,
            //    ShortNewsFromRssSource = n.ShortNewsFromRssSource,
            //    ImageUrl = n.ImageUrl,
            //    PublicationDate = n.PublicationDate,
            //    Author = n.Author,
            //    Category = n.Category
            //}).ToList();


            var pageInfo = new PageInfo(page, count);

            var newsListsWithPagination = new NewsListWithPagination()
            {
                NewsLists = newsList,
                Pagination = pageInfo
            };



            return PartialView("_NewsListsWithPagination", newsListsWithPagination);
        }


        // GET: News/Details/5
        public async Task<IActionResult> Read(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsWithRss = await _newsService
                .GetNewsWithRssSourceNameById(id.Value);

            if (newsWithRss == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<NewsWithCommentsViewModel>(newsWithRss);
            model.TotalComments = await _commentService.GetNumberOfCommentsByNewsId(newsWithRss.Id);

            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["RssSourceName"] = new SelectList(await _rssSourceService.GetAllRssSources(), "Id", "Name");
            return View();
        }

        // POST: News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(News news)
        {
            if (ModelState.IsValid)
            {
                var newsDto = _mapper.Map<NewsDto>(news);
                newsDto.Id = Guid.NewGuid();
                newsDto.PublicationDate = DateTime.Now.ToUniversalTime();
                try
                {
                    await _newsService.Add(newsDto);
                }
                catch (Exception e)
                {
                    Log.Logger.Error("News can\'t be added");
                }
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["RssSourceName"] = new SelectList( await _rssSourceService.GetAllRssSources(), "Id", "Name", news.RssSourceId);
            return View();
        }

        // GET: News/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _newsService.GetNewsById((Guid)id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["RssSourceName"] = new SelectList(await _rssSourceService.GetAllRssSources(), "Id", "Name", news.RssSourceId);
            var model = _mapper.Map<EditNewsViewModel>(news);
            return View(model);
        }

        // POST: News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(EditNewsViewModel news)
        {
            if (ModelState.IsValid)
            {
                var newsDto = _mapper.Map<NewsDto>(news);
                try
                {
                    _ = await _newsService.Update(newsDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Log.Logger.Error("News can\'t be updated");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RssSourceName"] = new SelectList(await _rssSourceService.GetAllRssSources(), "Id", "Name", news.RssSourceId);
            return View(news);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _newsService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        //private bool NewsExists(Guid id)
        //{
        //    return _context.News.Any(e => e.Id == id);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
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
