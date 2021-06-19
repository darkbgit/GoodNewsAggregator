using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using GoodNewsAggregator.DAL.Core;
using GoodNewsAggregator.DAL.Core.Entities;
using GoodNewsAggregator.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GoodNewsAggregator.DAL.Repositories.Implementation.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        protected readonly GoodNewsAggregatorContext Db;
        protected readonly DbSet<T> Table;

        protected Repository(GoodNewsAggregatorContext db)
        {
            Db = db;
            Table = Db.Set<T>();
        }

        public async Task<T> GetById(Guid id)
        {
            return await Table.FirstOrDefaultAsync(item => item.Id.Equals(id));
        }

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            var result = Table.Where(predicate);
            if (includes.Any())
            {
                result = includes
                    .Aggregate(result,
                        (current, include)
                            => current.Include(include));
            }

            return result.AsNoTracking();
        }

        public IQueryable<T> GetAll()
        {
            return Table.AsNoTracking();
        }

        public async Task Add(T element)
        {
            await Table.AddAsync(element);
        }

        public async Task AddRange(IEnumerable<T> elements)
        {
            await Table.AddRangeAsync(elements);
        }

        public void Update(T element)
        {
            Table.Update(element);
        }

        public async Task Remove(Guid id)
        {
            var entity = await GetById(id);
            Table.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> elements)
        {
            Table.RemoveRange(elements);
        }

        public void Dispose()
        {
            Db?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
