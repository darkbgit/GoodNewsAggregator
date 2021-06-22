using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.Services.Interfaces;

namespace GoodNewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var news = await _newsService.GetNewsById(id);

            //if (news == null)
            //{
            //    return NotFound();
            //}

            return Ok(news);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var news = await _newsService.GetNewsBySourceId(null);

            return Ok(news);
        }
    }
}
