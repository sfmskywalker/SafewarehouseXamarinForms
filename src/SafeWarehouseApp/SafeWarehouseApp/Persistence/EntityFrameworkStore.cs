using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SafeWarehouseApp.Models;

namespace SafeWarehouseApp.Persistence
{
    public class EntityFrameworkStore<T> : IStore<T> where T : class, IEntity
    {
        private readonly IMapper _mapper;
        private readonly SemaphoreSlim _semaphore = new(1);

        public EntityFrameworkStore(IDbContextFactory<SafeWarehouseContext> factory, IMapper mapper)
        {
            _mapper = mapper;
            Factory = factory;
        }

        protected IDbContextFactory<SafeWarehouseContext> Factory { get; }

        public async Task SaveAsync(T entity, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                await DoWork(async dbContext =>
                {
                    var dbSet = dbContext.Set<T>();
                    var existingEntity = await dbSet.FindAsync(new object[] { entity.Id }, cancellationToken);
                    
                    if (existingEntity == null)
                    {
                        await dbSet.AddAsync(entity, cancellationToken);
                        existingEntity = entity;
                    }
                    else
                    {
                        existingEntity = _mapper.Map(entity, existingEntity);
                    }

                    OnSaving(dbContext, existingEntity);
                }, cancellationToken);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
        {
            await DoWork(async dbContext =>
            {
                var dbSet = dbContext.Set<T>();
                await dbSet.AddAsync(entity, cancellationToken);
                OnSaving(dbContext, entity);
            }, cancellationToken);
        }

        public async Task AddManyAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            var list = entities.ToList();

            await DoWork(async dbContext =>
            {
                var dbSet = dbContext.Set<T>();
                await dbSet.AddRangeAsync(list, cancellationToken);

                foreach (var entity in list)
                    OnSaving(dbContext, entity);
            }, cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DoWork(dbContext =>
            {
                var dbSet = dbContext.Set<T>();
                dbSet.Attach(entity);
                dbContext.Entry(entity).State = EntityState.Modified;
                OnSaving(dbContext, entity);
            }, cancellationToken);
        }

        //public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) => await DoWorkOnSet(async dbSet => await dbSet.DeleteByKeyAsync(cancellationToken, entity.Id), cancellationToken);
        
        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default) => await DoWorkOnSet(async dbSet => await dbSet.SingleDeleteAsync(entity, cancellationToken), cancellationToken);

        public virtual async Task<int> DeleteManyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await DoWorkOnSet(async dbSet => await dbSet.Where(filter).DeleteFromQueryAsync(cancellationToken), cancellationToken);
        }

        public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? orderBy = default, IPaging? paging = default, CancellationToken cancellationToken = default)
        {
            return await DoWork(async dbContext =>
            {
                var dbSet = dbContext.Set<T>();
                var queryable = dbSet.Where(filter);

                if (orderBy != null) queryable = orderBy(queryable);

                if (paging != null)
                    queryable = queryable.Skip(paging.Skip).Take(paging.Take);

                return (await queryable.ToListAsync(cancellationToken)).Select(x => ReadShadowProperties(dbContext, x)).ToList();
            }, cancellationToken);
        }

        private T ReadShadowProperties(SafeWarehouseContext dbContext, T entity)
        {
            OnLoading(dbContext, entity);
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default) =>
            await DoWorkOnSet(async dbSet => await dbSet.CountAsync(filter, cancellationToken), cancellationToken);

        public async Task<T?> FindAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await DoWork(async dbContext =>
            {
                var dbSet = dbContext.Set<T>();
                var entity = await dbSet.FirstOrDefaultAsync(filter, cancellationToken);
                return entity != null ? ReadShadowProperties(dbContext, entity) : default;
            }, cancellationToken);
        }

        protected ValueTask DoWorkOnSet(Func<DbSet<T>, ValueTask> work, CancellationToken cancellationToken) => DoWork(dbContext => work(dbContext.Set<T>()), cancellationToken);
        protected ValueTask<TResult> DoWorkOnSet<TResult>(Func<DbSet<T>, ValueTask<TResult>> work, CancellationToken cancellationToken) => DoWork(dbContext => work(dbContext.Set<T>()), cancellationToken);
        protected ValueTask DoWorkOnSet(Action<DbSet<T>> work, CancellationToken cancellationToken) => DoWork(dbContext => work(dbContext.Set<T>()), cancellationToken);

        protected async ValueTask DoWork(Func<SafeWarehouseContext, ValueTask> work, CancellationToken cancellationToken)
        {
            await using var dbContext = Factory.CreateDbContext();
            await work(dbContext);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        protected async ValueTask<TResult> DoWork<TResult>(Func<SafeWarehouseContext, ValueTask<TResult>> work, CancellationToken cancellationToken)
        {
            await using var dbContext = Factory.CreateDbContext();
            var result = await work(dbContext);
            await dbContext.SaveChangesAsync(cancellationToken);
            return result;
        }

        protected async ValueTask DoWork(Action<SafeWarehouseContext> work, CancellationToken cancellationToken)
        {
            await using var dbContext = Factory.CreateDbContext();
            work(dbContext);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        protected virtual void OnSaving(SafeWarehouseContext dbContext, T entity) => OnSaving(entity);

        protected virtual void OnSaving(T entity)
        {
        }

        protected virtual void OnLoading(SafeWarehouseContext dbContext, T entity) => OnLoading(entity);

        protected virtual void OnLoading(T entity)
        {
        }
    }
}