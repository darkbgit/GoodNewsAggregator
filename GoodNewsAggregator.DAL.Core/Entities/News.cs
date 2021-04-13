using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class News : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSourse { get; set; }
        public string? ImageUrl { get; set; }
        public string? PublicationDate { get; set; }

        public Guid RssSourseId { get; set; }
        public virtual RssSourse RssSourse { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
