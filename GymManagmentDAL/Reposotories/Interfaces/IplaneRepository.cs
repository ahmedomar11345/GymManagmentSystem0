using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    public interface IplaneRepository
    {
        //getall
        IEnumerable<Plane> GetAll();
        //getbyId
        Plane? GetById(int id);
        //update
        int Update(Plane plane); 
    }
}
