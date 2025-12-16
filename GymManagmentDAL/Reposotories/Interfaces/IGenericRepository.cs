using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        //get all
        IEnumerable<TEntity> GetAll(Func<TEntity,bool>? condition = null);
        //get by id
        TEntity? GetById(int id);
        //add
        int Add(TEntity entity);
        //update
        int Update(TEntity entity);
        //delete
        int Delete(TEntity entity);
    }
}
