using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Persistence
{
    public interface IStore<T> where T : IEntity
    {
        Task SaveAsync(T entity, CancellationToken cancellationToken = default);
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> DeleteManyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? orderBy = default, IPaging? paging = default, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);
        Task<T?> FindAsync(Expression<Func<T, bool>> specification, CancellationToken cancellationToken = default);
    }

    public static class StoreExtensions
    {
        public static Task<T?> FindAsync<T>(this IStore<T> store, string id, CancellationToken cancellationToken = default) where T : IEntity => store.FindAsync(x => x.Id == id, cancellationToken);

        public static Task<IEnumerable<T>> ListAsync<T>(this IStore<T> store, Func<IQueryable<T>, IQueryable<T>>? orderBy = default, IPaging? paging = default, CancellationToken cancellationToken = default) where T : IEntity =>
            store.FindManyAsync(x => true, orderBy, paging, cancellationToken);

        public static async Task DeleteManyAsync<T>(this IStore<T> store, IEnumerable<T> items, CancellationToken cancellationToken = default) where T : IEntity
        {
            var ids = items.Select(x => x.Id).ToList();
            await store.DeleteManyAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}