using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;

namespace GoodNewsAggregator.Core.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetByNewsId(Guid id);

        Task Add(CommentDto comment);

        Task<int> GetNumberOfCommentsByNewsId(Guid id);
    }
}
