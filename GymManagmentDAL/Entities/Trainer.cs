using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Entities
{
    public class Trainer : GymUser
    {
        // shift times
        public TimeSpan ShiftStart { get; set; } = new TimeSpan(9, 0, 0); // Default 9 AM
        public TimeSpan ShiftEnd { get; set; } = new TimeSpan(17, 0, 0);  // Default 5 PM
        public string? Photo { get; set; }

        public int? SpecialtyId { get; set; }
        public TrainerSpecialty? Specialty { get; set; }

        #region Trainer - Session
        public ICollection<Session> Sessions { get; set; } = null!; 
        #endregion

        #region Trainer - Attendance
        public ICollection<TrainerAttendance> Attendances { get; set; } = new HashSet<TrainerAttendance>();
        #endregion
    }
}
