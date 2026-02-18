using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class Member : GymUser
    {
        //joindate == createdat of baseentity
        public string Photo { get; set; } = null!; 

        #region Member - HealthRecord
        public HealthRecord HealthRecord { get; set; } = null!; 
        public ICollection<HealthProgress> HealthProgresses { get; set; } = new List<HealthProgress>();
        #endregion

        #region Member - MemberShip
        public ICollection<MemberShip> Memberships { get; set; } = null!;
        #endregion

        #region Member - MemberSession
        public ICollection<MemberSession> MemberSessions { get; set; } = null!;

        #endregion

    }
}
