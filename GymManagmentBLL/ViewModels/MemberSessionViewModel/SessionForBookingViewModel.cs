using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberSessionViewModel
{
    public class SessionForBookingViewModel
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string TrainerName { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public int AvailableSlots => Capacity - BookedCount;
        public string Status { get; set; } = null!; // Upcoming or Ongoing
    }
}
