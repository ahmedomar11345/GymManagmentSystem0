using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class TrainerAttendance : BaseEntity
    {
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;
        
        public DateTime Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        public double TotalHours { get; set; }
        public int DelayMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        
        public string? Notes { get; set; }
    }
}
