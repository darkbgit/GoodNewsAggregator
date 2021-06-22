using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CommentsController(ICommentService commentService,
            UserManager<User> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> List(Guid newsId)
        {
            var comments = await _commentService.GetByNewsId(newsId);

            if (comments != null)
            {
                comments.ToList().ForEach(c => 
                {
                    c.UserName =
                       _userManager.FindByIdAsync(c.UserId.ToString()).Result.UserName;
                });
            }

            var commentListViewModel = new CommentsListViewModel()
            {
                Comments = comments,
                NewsId = newsId
            };
            return View(commentListViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateCommentViewModel model)
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
                    UserName = user.UserName
                };

                await _commentService.Add(commentDto);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<int> GetTotalComments(Guid newsId)
        {
            return await _commentService.GetNumberOfCommentsByNewsId(newsId);
        }
    }
}
