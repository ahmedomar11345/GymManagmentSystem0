using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>>? condition = null);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null);
        
        PagedResult<TEntity> GetPaged(
            int pageNumber, 
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
            
        Task<PagedResult<TEntity>> GetPagedAsync(
            int pageNumber, 
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null);
        
        TEntity? GetById(int id);
        Task<TEntity?> GetByIdAsync(int id);
        
        bool Exists(Expression<Func<TEntity, bool>> condition);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> condition);
        
        int Count(Expression<Func<TEntity, bool>>? condition = null);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null);
        
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
    }
    
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
