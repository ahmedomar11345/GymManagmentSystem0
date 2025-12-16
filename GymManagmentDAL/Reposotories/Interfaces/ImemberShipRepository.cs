using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    internal interface ImemberShipRepository
    {
        //getall
        IEnumerable<MemberShip> GetAll();
        //getbyId
        MemberShip? GetById(int id);
        //add
        int Add(MemberShip memberShip);
        //update
        int Update(MemberShip memberShip);
        //delete
        int Delete(int id);
    }
}
