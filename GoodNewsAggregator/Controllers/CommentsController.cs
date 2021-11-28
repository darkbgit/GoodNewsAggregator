using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.Models.ViewModels.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GoodNewsAggregator.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public CommentsController(ICommentService commentService,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _commentService = commentService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> ListAsync(Guid newsId)
        {
            var comments = await _commentService.GetByNewsId(newsId);

            if (comments == null) return default;

            var commentModels = comments
                .Select(async c =>  new OneCommentViewModel
                {
                    Id = c.Id,
                    PublicationDate = c.PublicationDate,
                    Text = c.Text,
                    UserName = (await _userManager.FindByIdAsync(c.UserId.ToString())).UserName
                })
                .ToList();

            var cm = await Task.WhenAll(commentModels);

            var commentListViewModel = new CommentsListViewModel()
            {
                Comments = cm.ToList(),
                NewsId = newsId
            };
            return View(commentListViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                var commentDto = new CommentDto()
                {
                    Id = Guid.NewGuid(),
                    NewsId = model.NewsId,
                    Text = model.CommentText,
                    PublicationDate = DateTime.Now.ToUniversalTime(),
                    UserId = user.Id,
                };

                await _commentService.Add(commentDto);
            }
            return Ok();
        }

        [HttpPost]
        public async Task<int> GetTotalCommentsAsync(Guid newsId)
        {
            return await _commentService.GetNumberOfCommentsByNewsId(newsId);
        }
    }
}
