using System;

namespace GymManagmentDAL.Entities
{
    public class CheckIn : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime CheckInTime { get; set; }
        public string? Notes { get; set; }
    }
}
