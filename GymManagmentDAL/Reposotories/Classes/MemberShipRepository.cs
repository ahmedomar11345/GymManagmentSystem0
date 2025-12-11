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
    internal class MemberShipRepository : ImemberShipRepostory
    {
        private readonly GymDBContext _dbContext = new GymDBContext();
        public int Add(MemberShip memberShip)
        {
            _dbContext.MemberShips.Add(memberShip);
            return _dbContext.SaveChanges();
        }

        public int Delete(int id)
        {
            var memberShip = _dbContext.MemberShips.Find(id);
            if (memberShip is null)
                return 0;
            _dbContext.MemberShips.Remove(memberShip);
            return _dbContext.SaveChanges();
        }

        public IEnumerable<MemberShip> GetAll()=> _dbContext.MemberShips.ToList();

        public MemberShip? GetById(int id)=> _dbContext.MemberShips.Find(id);

        public int Update(MemberShip memberShip)
        {
            _dbContext.MemberShips.Update(memberShip);
            return _dbContext.SaveChanges();
        }
    }
}
