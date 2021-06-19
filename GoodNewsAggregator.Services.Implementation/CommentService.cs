using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GoodNewsAggregator.Core.DTOs;
using GoodNewsAggregator.Core.Services.Interfaces;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.Services.Implementation
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task Add(CommentDto comment)
        {
            await _unitOfWork.Comments.Add(_mapper.Map<Comment>(comment));
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<CommentDto>> GetByNewsId(Guid id)
        {
            return await _unitOfWork.Comments
                .FindBy(comment => comment.NewsId.Equals(id))
                .Select(comment => _mapper.Map<CommentDto>(comment)).ToListAsync();
        }
    }
}
