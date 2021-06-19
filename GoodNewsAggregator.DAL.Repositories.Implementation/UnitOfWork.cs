using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GoodNewsAggregatorContext _db;

        public UnitOfWork(GoodNewsAggregatorContext db,
            IRepository<News> newsRepository,
            IRepository<RssSource> rssRepository,
            IRepository<Comment> commentsRepository)
        {
            _db = db;
            News = newsRepository;
            RssSources = rssRepository;
            Comments = commentsRepository;
        }

        public IRepository<News> News { get; }

        public IRepository<RssSource> RssSources { get; }

        public IRepository<User> Users { get; }

        public IRepository<Comment> Comments { get; }

        public void Dispose()
        {
            _db?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
