using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class Comment : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid NewsId { get; set; }
        public virtual News News { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
