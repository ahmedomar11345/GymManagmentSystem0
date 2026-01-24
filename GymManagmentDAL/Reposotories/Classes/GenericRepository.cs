using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDBContext _dbContext;

        public GenericRepository(GymDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(TEntity entity)=> _dbContext.Set<TEntity>().Add(entity);


        public void Delete(TEntity entity)=> _dbContext.Set<TEntity>().Remove(entity);


        public IEnumerable<TEntity> GetAll(Func<TEntity, bool>? condition = null)
        {
            if (condition is null)
                return _dbContext.Set<TEntity>().AsNoTracking().ToList();
            else
                return _dbContext.Set<TEntity>().AsNoTracking().Where(condition).ToList();
        }

        public TEntity? GetById(int id)=> _dbContext.Set<TEntity>().Find(id);

        public void Update(TEntity entity)=> _dbContext.Set<TEntity>().Update(entity);

    }
}
