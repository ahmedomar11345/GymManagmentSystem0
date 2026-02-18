using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class MemberShip : BaseEntity
    {
        //startdate ==> created at of baseentity
        public DateTime EndDate { get; set; }
        public bool IsFrozen { get; set; } = false;
        public DateTime? FreezeStartDate { get; set; }
        public DateTime? FreezeEndDate { get; set; }

        public string Status { get
            {
                if (IsFrozen && FreezeStartDate <= DateTime.Now && (FreezeEndDate == null || FreezeEndDate >= DateTime.Now))
                    return "Frozen";
                if(EndDate > DateTime.Now)
                    return "Active";
                else
                    return "Expired";
            }
        }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int PlanId { get; set; }
        public Plane Plan { get; set; } = null!;
    }
}
