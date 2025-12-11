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
    internal class PlaneRepository : IplaneRepostory
    {
        private readonly GymDBContext _dbContext = new GymDBContext();
        public int Add(Plane plane)
        {
            _dbContext.Planes.Add(plane);
            return _dbContext.SaveChanges();
        }

        public int Delete(int id)
        {
            var plane = _dbContext.Planes.Find(id);
            if (plane is null)
                return 0;
            _dbContext.Planes.Remove(plane);
            return _dbContext.SaveChanges();
        }

        public IEnumerable<Plane> GetAll()=> _dbContext.Planes.ToList();

        public Plane? GetById(int id)=> _dbContext.Planes.Find(id);

        public int Update(Plane plane)
        {
            _dbContext.Planes.Update(plane);
            return _dbContext.SaveChanges();

        }
    }
}
