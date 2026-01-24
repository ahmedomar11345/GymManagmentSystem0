using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly Dictionary<Type, object> _repositories = new();
        private readonly GymDBContext _dBContext;

        public UnitOfWork(GymDBContext dBContext, ISessionRepository sessionRepository)
        {
            _dBContext = dBContext;
            this.sessionRepository = sessionRepository;
        }

        public ISessionRepository sessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //TEntity ==> member
            var Entitytype = typeof(TEntity);
            if(_repositories.TryGetValue(Entitytype,out var repo))
                return (IGenericRepository<TEntity>)repo;
            var Newrepo = new GenericRepository<TEntity>(_dBContext);
            _repositories.Add(Entitytype, Newrepo);
            return Newrepo;

        }

        public int SaveChanges()
        {
            return _dBContext.SaveChanges();
        }

        public bool ExecuteInTransaction(Func<bool> operation)
        {
            using var transaction = _dBContext.Database.BeginTransaction();
            try
            {
                var success = operation();
                if (!success)
                {
                    transaction.Rollback();
                    return false;
                }

                _dBContext.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
