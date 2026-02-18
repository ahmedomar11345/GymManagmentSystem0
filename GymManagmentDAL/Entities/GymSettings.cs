using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class GymSettings : BaseEntity
    {
        public string GymName { get; set; } = "IronPulse Gym";
        public string Phone { get; set; } = "+20 123 456 789";
        public string Address { get; set; } = "Cairo";
        public string Country { get; set; } = "Egypt";
        public string? Email { get; set; }
        public string? LogoUrl { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? FooterText { get; set; }
    }
}
