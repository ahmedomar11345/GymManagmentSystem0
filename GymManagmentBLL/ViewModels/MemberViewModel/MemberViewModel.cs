using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string? Photo { get; set; }
        public string Name { get; set; } = null!;   
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? PlaneName { get; set; } 
        public string? DateOfBirth { get; set; }
        public string? MembershipStartDate { get; set; } 
        public string? MembershipEndDate { get; set; }
        public string? Address { get; set; }
        public DateTime JoinDate { get; set; }
        public int? SessionsBooked { get; set; }
        public string? AccessKey { get; set; }

        // Health Record
        public HealthRecordViewModel? HealthRecord { get; set; }
        public IEnumerable<GymManagmentBLL.Service.Interfaces.CheckInViewModel>? CheckInHistory { get; set; }
    }
}
