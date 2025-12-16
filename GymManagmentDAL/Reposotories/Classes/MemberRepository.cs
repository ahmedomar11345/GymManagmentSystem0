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
    internal class MemberRepository : GenericRepository<Member>
    {
        private readonly GymDBContext _dbContext;

        //Ask clr inject object from GymDBContext
        public MemberRepository(GymDBContext dbContext):base(dbContext)
        {
           
        }
        //public int Add(Member member)
        //{
        //    _dbContext.Members.Add(member);
        //    return _dbContext.SaveChanges();
        //}

        
        //public int Delete(Member entity)
        //{
        //    _dbContext.Members.Remove(entity);
        //    return _dbContext.SaveChanges();
        //}

        //public IEnumerable<Member> GetAll() => _dbContext.Members.ToList();

        //public Member? GetById(int id)=> _dbContext.Members.Find(id);

        //public int Update(Member member)
        //{
        //    _dbContext.Members.Update(member);
        //    return _dbContext.SaveChanges();
        //}
    }
}
