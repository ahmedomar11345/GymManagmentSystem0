using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    internal class Member : GymUser
    {
        //joindate == createdat of baseentity
        public string? Photo { get; set; }

        #region Member - HealthRecord
        public HealthRecord HealthRecord { get; set; } = null!; 
        #endregion

        #region Member - MemberShip
        public ICollection<MemberShip> Memberships { get; set; } = null!;
        #endregion

        #region Member - MemberSession
        public ICollection<MemberSession> MemberSessions { get; set; } = null!;

        #endregion

    }
}
