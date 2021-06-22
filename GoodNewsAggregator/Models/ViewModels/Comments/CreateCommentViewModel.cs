using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Comments
{
    public class CreateCommentViewModel
    {
        public Guid NewsId { get; set; }

        [Required(ErrorMessage = "Введите коментарий")]
        [StringLength(10000, MinimumLength = 3, ErrorMessage = "Длина коментария должна быть от 3 до 10000 символов")]
        public string CommentText { get; set; }
    }
}
