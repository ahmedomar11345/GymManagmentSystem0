using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    internal class MemberViewModel
    {
        public int Id { get; set; }
        public string? Phote { get; set; }
        public string Name { get; set; } = null!;   
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string? PlaneName { get; set; } 
        public string? DateOfBirth { get; set; }
        public string? MembershipStartDate { get; set; } 
        public string? MembershipEndDate { get; set; }
        public string? Address { get; set; }


    }
}
