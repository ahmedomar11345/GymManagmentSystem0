using System;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    public class HealthProgressViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
        public decimal BMI { get; set; }
        public decimal? BodyFatPercentage { get; set; }
        public decimal? MuscleMass { get; set; }
        public DateTime ProgressDate { get; set; }
        public string? Note { get; set; }
    }
}
