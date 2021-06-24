using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core.Enums;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class News : IBaseEntity
    {
        public Guid Id { get; set; }
        
        public string Title { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public string ShortNewsFromRssSource { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? PublicationDate { get; set; }
        public string? Category { get; set; }
        public string? Author { get; set; }
        public double Rating { get; set; }
        public NewsStatus Status { get; set; }

        public Guid RssSourceId { get; set; }
        public virtual RssSource RssSource { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
