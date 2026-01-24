using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberSessionViewModel
{
    public class SessionMembersViewModel
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; } = null!;
        public IEnumerable<BookingViewModel> Bookings { get; set; } = null!;
    }
}
