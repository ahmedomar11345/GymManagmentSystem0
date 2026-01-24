using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace GymManagmentDAL.Reposotories.Interfaces
{
    public interface IUnitOfWork
    {
        public ISessionRepository sessionRepository { get; }
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity,new();

        int SaveChanges();

        // Execute an operation inside a database transaction. The provided operation should perform repository actions
        // and return true to indicate success (will commit) or false to indicate failure (will rollback).
        bool ExecuteInTransaction(Func<bool> operation);
    }
}
