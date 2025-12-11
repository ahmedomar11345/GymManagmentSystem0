using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Interfaces
{
    internal interface ImemberRepostory
    {
        //getall
        IEnumerable<Member> GetAll();
        //getbyId
        Member? GetById(int id);
        //add
        int Add(Member member);
        //update
        int Update(Member member);
        //delete
        int Delete(int id);
    }
}
