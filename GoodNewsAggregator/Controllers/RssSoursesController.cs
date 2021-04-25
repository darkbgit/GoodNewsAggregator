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
    public class RssSoursesController : Controller
    {
        private readonly GoodNewsAggregatorContext _context;

        public RssSoursesController(GoodNewsAggregatorContext context)
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
                Log.Error("RssSoursesController Details sourse = null");
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
        public async Task<IActionResult> Create(RssSource sourse)
        {
            if (ModelState.IsValid)
            {
                sourse.Id = Guid.NewGuid();
                await _context.RssSources.AddAsync(sourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(sourse);
        }
    }
}
