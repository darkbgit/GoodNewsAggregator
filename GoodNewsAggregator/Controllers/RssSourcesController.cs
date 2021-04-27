using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace GoodNewsAggregator.Controllers
{
    public class RssSourcesController : Controller
    {
        private readonly GoodNewsAggregatorContext _context;

        public RssSourcesController(GoodNewsAggregatorContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.RssSources.ToListAsync();
            
            return View(model);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                Log.Error("Method Details id = null");
                return NotFound();
            }

            var source = await _context.RssSources
                .FirstOrDefaultAsync(m => m.Id == id);
            if (source == null)
            {
                Log.Error("RssSourcesController Details source = null");
                return NotFound();
            }

            return View(source);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RssSource source)
        {
            if (ModelState.IsValid)
            {
                source.Id = Guid.NewGuid();
                await _context.RssSources.AddAsync(source);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(source);
        }
    }
}
