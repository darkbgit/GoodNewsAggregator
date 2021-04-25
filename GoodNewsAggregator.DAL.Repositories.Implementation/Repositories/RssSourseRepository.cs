using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;

namespace GoodNewsAggregator.DAL.Repositories.Implementation.Repositories
{
    public class RssSourseRepository : Repository<RssSource>
    {
        public RssSourseRepository(GoodNewsAggregatorContext context)
            : base(context)
        {
        }
    }
}
