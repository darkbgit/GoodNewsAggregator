using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Repositories.Implementation.Repositories
{
    public class UserRepository : Repository<User>
    {
        public UserRepository(GoodNewsAggregatorContext context)
            : base(context)
        {

        }
    }
}
