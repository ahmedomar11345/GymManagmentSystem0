using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    internal interface IplaneRepostory
    {
        //getall
        IEnumerable<Plane> GetAll();
        //getbyId
        Plane? GetById(int id);
        //add
        int Add(Plane plane);
        //update
        int Update(Plane plane); 
        //delete
        int Delete(int id);
    }
}
