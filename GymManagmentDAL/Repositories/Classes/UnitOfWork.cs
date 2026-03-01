using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly GymDBContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;
        private bool _disposed = false;

        public UnitOfWork(
            GymDBContext dbContext,
            ISessionRepository sessionRepository,
            IMemberShipRepository memberShipRepository,
            IMemberSessionRepository memberSessionRepository,
            IMemberRepository memberRepository,
            ITrainerRepository trainerRepository,
            ITrainerSpecialtyRepository trainerSpecialtyRepository,
            INotificationRepository notificationRepository,
            ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
            MemberShipRepository = memberShipRepository;
            MemberSessionRepository = memberSessionRepository;
            MemberRepository = memberRepository;
            TrainerRepository = trainerRepository;
            TrainerSpecialtyRepository = trainerSpecialtyRepository;
            NotificationRepository = notificationRepository;
            _logger = logger;
        }

        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MemberShipRepository { get; }
        public IMemberSessionRepository MemberSessionRepository { get; }
        public IMemberRepository MemberRepository { get; }
        public ITrainerRepository TrainerRepository { get; }
        public ITrainerSpecialtyRepository TrainerSpecialtyRepository { get; }
        public INotificationRepository NotificationRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var entityType = typeof(TEntity);
            
            if (_repositories.TryGetValue(entityType, out var repo))
                return (IGenericRepository<TEntity>)repo;
            
            var newRepo = new GenericRepository<TEntity>(_dbContext);
            _repositories.Add(entityType, newRepo);
            return newRepo;
        }

        public int SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving changes asynchronously to database");
                throw;
            }
        }

        public bool ExecuteInTransaction(Func<bool> operation)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            try
            {
                return strategy.Execute<bool>(() =>
                {
                    using var transaction = _dbContext.Database.BeginTransaction();
                    try
                    {
                        var success = operation();
                        if (!success)
                        {
                            transaction.Rollback();
                            _logger.LogWarning("Transaction rolled back due to operation returning false");
                            return false;
                        }

                        _dbContext.SaveChanges();
                        transaction.Commit();
                        _logger.LogDebug("Transaction committed successfully");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogWarning(ex, "Transaction attempt failed, might retry...");
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction failed after retries.");
                return false;
            }
        }

        public async Task<bool> ExecuteInTransactionAsync(Func<Task<bool>> operation)
        {
            var strategy = _dbContext.Database.CreateExecutionStrategy();
            try
            {
                return await strategy.ExecuteAsync<bool>(async () =>
                {
                    await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var success = await operation();
                        if (!success)
                        {
                            await transaction.RollbackAsync();
                            _logger.LogWarning("Async transaction rolled back due to operation returning false");
                            return false;
                        }

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        _logger.LogDebug("Async transaction committed successfully");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogWarning(ex, "Async transaction attempt failed, might retry...");
                        throw; 
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Async transaction failed after retries.");
                return false;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
