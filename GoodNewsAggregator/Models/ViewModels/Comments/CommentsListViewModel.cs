using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.Core.DTOs;

namespace GoodNewsAggregator.Models.ViewModels.Comments
{
    public class CommentsListViewModel
    {
        public Guid NewsId { get; set; }

        public IEnumerable<OneCommentViewModel> Comments { get; set; }
    }
}
