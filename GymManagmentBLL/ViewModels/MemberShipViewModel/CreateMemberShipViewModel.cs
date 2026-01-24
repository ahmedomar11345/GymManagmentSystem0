using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberShipViewModel
{
    public class CreateMemberShipViewModel
    {
        [Required(ErrorMessage = "Member is required")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Plan is required")]
        [Display(Name = "Plan")]
        public int PlanId { get; set; }
    }
}
