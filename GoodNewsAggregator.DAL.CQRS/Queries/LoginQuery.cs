using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core.Entities;
using MediatR;

namespace GoodNewsAggregator.DAL.CQRS.Queries
{
    public class LoginQuery : IRequest<User>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
