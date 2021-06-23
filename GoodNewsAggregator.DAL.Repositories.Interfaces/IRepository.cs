﻿using GoodNewsAggregator.DAL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GoodNewsAggregator.DAL.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable where T : class, IBaseEntity
    {
        
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<T> Get(Guid id);
        IQueryable<T> GetAll();
        
        Task Add (T entity);
        Task AddRange(IEnumerable<T> entities);

        Task Update(T entity);

        Task Remove(Guid id);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
