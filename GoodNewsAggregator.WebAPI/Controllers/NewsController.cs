using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;

using MediatR;

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

        [HttpGet("/{id:guid}")]
        public async Task<ActionResult<NewsDto>> Get(Guid id)
        {
            return await _newsService.GetNewsById(id);
        }

        [HttpGet]
        public async Task<IEnumerable<NewsDto>> Get()
        {
            return await _newsService.GetAllNews();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] NewsDto news)
        {
            return await _newsService.Add(news);
        }

        [HttpPut]
        public async Task<ActionResult<int>> Update([FromBody] NewsDto news)
        {
            return await _newsService.Update(news);
        }

        [HttpDelete("/{id:guid}")]
        public async Task<ActionResult<int>> Delete(Guid id)
        {
            return await _newsService.Delete(id);
        }
    }
}
