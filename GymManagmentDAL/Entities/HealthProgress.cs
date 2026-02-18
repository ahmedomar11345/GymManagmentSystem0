using System;

namespace GymManagmentDAL.Entities
{
    public class HealthProgress : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal BMI { get; set; }
        
        public decimal? BodyFatPercentage { get; set; }
        public decimal? MuscleMass { get; set; }
        
        public DateTime ProgressDate { get; set; } = DateTime.Now;
        public string? Note { get; set; }
    }
}
