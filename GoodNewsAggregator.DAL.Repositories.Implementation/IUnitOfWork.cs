using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Repositories.Implementation
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<News> News { get; }
        IRepository<RssSource> RssSources { get; }
        IRepository<User> Users { get; }
        IRepository<Comment> Comments { get; }

        Task<int> SaveChangesAsync();
    }
}
