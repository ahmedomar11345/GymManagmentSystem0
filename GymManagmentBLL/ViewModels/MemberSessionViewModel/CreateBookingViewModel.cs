using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberSessionViewModel
{
    public class CreateBookingViewModel
    {
        [Required(ErrorMessage = "Member is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a member")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Session is required")]
        public int SessionId { get; set; }
    }
}
