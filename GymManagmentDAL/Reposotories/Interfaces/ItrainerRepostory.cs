using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    internal interface ItrainerRepostory
    {
        //get all
        IEnumerable<Trainer> GetAll();
        //get by id
        Trainer? GetById(int id);
        //add
        int Add(Trainer trainer);
        //update
        int Update(Trainer trainer);
        //delete
        int Delete(int id);

    }
}
