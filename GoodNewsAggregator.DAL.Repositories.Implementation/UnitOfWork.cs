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
        private readonly IRepository<News> _newsRepository;
        private readonly IRepository<RssSource> _rssRepository;
        private readonly IRepository<User> _userRepository;

        public UnitOfWork(GoodNewsAggregatorContext db,
            IRepository<News> newsRepository,
            IRepository<RssSource> rssRepository)
        {
            _db = db;
            _newsRepository = newsRepository;
            _rssRepository = rssRepository;
        }

        public IRepository<News> News => _newsRepository;
        public IRepository<RssSource> RssSources => _rssRepository;
        public IRepository<User> Users => _userRepository;

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
