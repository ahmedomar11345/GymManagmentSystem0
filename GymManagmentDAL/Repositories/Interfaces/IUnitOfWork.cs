using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ISessionRepository SessionRepository { get; }
        IMemberShipRepository MemberShipRepository { get; }
        IMemberSessionRepository MemberSessionRepository { get; }
        IMemberRepository MemberRepository { get; }
        ITrainerRepository TrainerRepository { get; }
        ITrainerSpecialtyRepository TrainerSpecialtyRepository { get; }
        INotificationRepository NotificationRepository { get; }
        
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        int SaveChanges();
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Execute an operation inside a database transaction.
        /// The provided operation should perform repository actions and return true to indicate success (will commit) or false to indicate failure (will rollback).
        /// </summary>
        bool ExecuteInTransaction(Func<bool> operation);
        
        /// <summary>
        /// Execute an async operation inside a database transaction.
        /// </summary>
        Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> operation);
    }
}
