using GoodNewsAggregator.DAL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        Task<T> GetById(Guid id);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);
        IQueryable<T> Get();

        Task Add(T element);
        Task AddRange(IEnumerable<T> elements);

        Task Update(T element);
        Task Remove(Guid id);
        Task RemoveRange(IEnumerable<T> elements);
    }
}
