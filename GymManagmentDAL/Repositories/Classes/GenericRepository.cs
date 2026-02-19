using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        protected readonly GymDBContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(GymDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? condition = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            
            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (condition is not null) query = query.Where(condition);
            return query.ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (condition is not null) query = query.Where(condition);
            return await query.ToListAsync();
        }

        public virtual PagedResult<TEntity> GetPaged(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            if (filter is not null) query = query.Where(filter);
            var totalCount = query.Count();
            query = orderBy is not null ? orderBy(query) : query.OrderBy(e => e.Id);
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<TEntity> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public virtual async Task<PagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            IQueryable<TEntity> query = _dbSet.AsNoTracking();
            if (filter is not null) query = query.Where(filter);
            var totalCount = await query.CountAsync();
            query = orderBy is not null ? orderBy(query) : query.OrderBy(e => e.Id);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<TEntity> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public virtual TEntity? GetById(int id) => _dbSet.Find(id);

        public virtual async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public virtual bool Exists(Expression<Func<TEntity, bool>> condition) => _dbSet.AsNoTracking().Any(condition);

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition) => await _dbSet.AsNoTracking().AnyAsync(condition);

        public virtual int Count(Expression<Func<TEntity, bool>>? condition = null) => condition is null ? _dbSet.AsNoTracking().Count() : _dbSet.AsNoTracking().Count(condition);

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null) => condition is null ? await _dbSet.AsNoTracking().CountAsync() : await _dbSet.AsNoTracking().CountAsync(condition);

        public virtual void Add(TEntity entity)
        {
            entity.CreatedAt = DateTime.Now;
            _dbSet.Add(entity);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.Now;
            await _dbSet.AddAsync(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) entity.CreatedAt = DateTime.Now;
            _dbSet.AddRange(entities);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) entity.CreatedAt = DateTime.Now;
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _dbSet.Update(entity);
        }

        public virtual void Delete(TEntity entity) => _dbSet.Remove(entity);

        public virtual void DeleteRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
    }
}
