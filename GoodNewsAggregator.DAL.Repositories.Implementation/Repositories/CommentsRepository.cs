using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;

namespace GoodNewsAggregator.DAL.Repositories.Implementation.Repositories
{
    public class CommentsRepository : Repository<Comment>
    {
        public CommentsRepository(GoodNewsAggregatorContext context)
            : base(context)
        {
        }
    }
}
