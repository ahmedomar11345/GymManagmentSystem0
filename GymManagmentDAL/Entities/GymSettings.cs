using System.ComponentModel.DataAnnotations;

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

        [Range(0.0, 1000000, ErrorMessage = "سعر الحصة لا يمكن أن يكون سالباً")]
        public decimal SessionPrice { get; set; } = 50;

        [Display(Name = "Walk-in Retention Days")]
        [Range(0, 3650, ErrorMessage = "Invalid retention days")]
        public int WalkInRetentionDays { get; set; } = 30;
    }
}
