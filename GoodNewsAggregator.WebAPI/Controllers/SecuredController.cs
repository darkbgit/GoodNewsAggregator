using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GoodNewsAggregator.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SecuredController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSecuredData()
        {
            return Ok("Secured data");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostSecuredData()
        {
            return Ok("Admin secured data");
        }
    }
}
