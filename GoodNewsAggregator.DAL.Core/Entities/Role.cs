using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public sealed class Role : IdentityRole<Guid>, IBaseEntity
    {
        public Role(string name)
            : base(name)
        {
            Id = Guid.NewGuid();
        }

    }
}
