using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;

namespace GoodNewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RssSourceController : ControllerBase
    {
        private readonly IRssSourceService _sourceService;

        public RssSourceController(IRssSourceService sourceService)
        {
            _sourceService = sourceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var source = await  _sourceService.GetRssSourceById(id);

            return Ok(source);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var source = await _sourceService.GetAllRssSources();

            return Ok(source);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RssSourceDto request)
        {
            var source = await _sourceService.GetAllRssSources();

            return Ok(source);
        }

        [HttpPut]
        public async Task<IActionResult> Put()
        {
            var source = await _sourceService.GetAllRssSources();

            return Ok(source);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch()
        {
            var source = await _sourceService.GetAllRssSources();

            return Ok(source);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var source = await _sourceService.GetAllRssSources();

            return Ok(source);
        }
    }
}
