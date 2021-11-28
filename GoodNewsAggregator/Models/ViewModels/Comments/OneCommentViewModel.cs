using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoodNewsAggregator.Models.ViewModels.Comments
{
    public class OneCommentViewModel
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
        public DateTime PublicationDate { get; set; }

        public string UserName { get; set; }
    }
}
