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
        [Required(ErrorMessage = "MemberRequired")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "PlanRequired")]
        [Display(Name = "Plan")]
        public int PlanId { get; set; }

        [Display(Name = "Send Email in Arabic")]
        public bool SendInArabic { get; set; } = true;
    }
}
