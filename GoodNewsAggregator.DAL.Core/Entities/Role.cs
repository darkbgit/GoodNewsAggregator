﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class Role : IdentityRole<Guid>, IBaseEntity
    {

    }
}
