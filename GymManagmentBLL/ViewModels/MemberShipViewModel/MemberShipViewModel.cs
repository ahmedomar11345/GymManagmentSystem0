using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberShipViewModel
{
    public class MemberShipViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public int PlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int DaysRemaining { get; set; }
        public int? SessionsRemaining { get; set; }
        public bool IsSessionBased { get; set; }
        public string? MemberPhoto { get; set; }
    }
}
