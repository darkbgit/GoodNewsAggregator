﻿using GoodNewsAggregator.DAL.Core;
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
        private readonly IRepository<RssSourse> _rssRepository;

        public UnitOfWork(GoodNewsAggregatorContext db,
            IRepository<News> newsRepository,
            IRepository<RssSourse> rssRepository)
        {
            _db = db;
            _newsRepository = newsRepository;
            _rssRepository = rssRepository;
        }

        public IRepository<News> News => _newsRepository;
        public IRepository<RssSourse> RssSourses => _rssRepository;

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
