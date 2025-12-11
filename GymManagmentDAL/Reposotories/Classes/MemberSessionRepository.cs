using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    internal class MemberSessionRepository : ImemberSessionRepostory
    {
        private readonly GymDBContext _dbContext = new GymDBContext();
        public int Add(MemberSession memberSession)
        {
            _dbContext.MemberSessions.Add(memberSession);
            return _dbContext.SaveChanges();
        }

        public int Delete(int id)
        {
            var memberSession = _dbContext.MemberSessions.Find(id);
            if (memberSession is null)
                return 0;
            _dbContext.MemberSessions.Remove(memberSession);
            return _dbContext.SaveChanges();
        }

        public IEnumerable<MemberSession> GetAll()=> _dbContext.MemberSessions.ToList();

        public MemberSession? GetById(int id)=> _dbContext.MemberSessions.Find(id);

        public int Update(MemberSession memberSession)
        {
            _dbContext.MemberSessions.Update(memberSession);
            return _dbContext.SaveChanges();

        }
    }
}
