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
    public class PlaneRepository : IplaneRepository
    {
        private readonly GymDBContext _dbContext;
        public PlaneRepository(GymDBContext dbContext)
        {
            _dbContext = dbContext;
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
