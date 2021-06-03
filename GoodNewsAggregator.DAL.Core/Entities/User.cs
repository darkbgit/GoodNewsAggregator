using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Core.Entities
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        //public Guid Id { get; set; }
        //public string Name { get; set; }
        //public string Email { get; set; }

        //public string? PasswordHash { get; set; }

        //public double MinimalRating { get; set; }
        public int Year { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}
