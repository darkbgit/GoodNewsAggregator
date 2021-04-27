using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;

namespace GoodNewsAggregator.DAL.Repositories.Implementation.Repositories
{
    public class RssSourceRepository : Repository<RssSource>
    {
        public RssSourceRepository(GoodNewsAggregatorContext context)
            : base(context)
        {
        }
    }
}
