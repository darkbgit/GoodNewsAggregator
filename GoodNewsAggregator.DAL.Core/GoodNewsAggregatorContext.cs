using GoodNewsAggregator.DAL.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GoodNewsAggregator.DAL.Core
{
    public class GoodNewsAggregatorContext : DbContext
    {
        public GoodNewsAggregatorContext(DbContextOptions<GoodNewsAggregatorContext> options)
            : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<RssSourse> RssSourses { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
