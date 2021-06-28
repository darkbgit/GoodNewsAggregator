using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core.Entities;

namespace GoodNewsAggregator.WebAPI.Auth
{
    public interface IJwtGenerator
    {
        string CreateToken(User user);
    }
}
