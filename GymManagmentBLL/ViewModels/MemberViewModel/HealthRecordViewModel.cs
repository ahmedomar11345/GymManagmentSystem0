using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    public class HealthRecordViewModel
    {
        [Required(ErrorMessage = "Height Is Required!")]
        [Range(minimum: 0.1, maximum: 300, ErrorMessage = "Height Must Be Greater Than 0 And Less Than 300!")]
        public decimal Height { get; set; } = default;

        [Required(ErrorMessage = "Weight Is Required!")]
        [Range(minimum: 0.1, maximum: 500, ErrorMessage = "Weight Must Be Greater Than 0 And Less Than 500!")]
        public decimal Weight { get; set; } = default;

        [Required(ErrorMessage = "Blood Type Is Required!")]
        [StringLength(maximumLength: 3, MinimumLength= 1, ErrorMessage = "Blood Type Must Be 3 Char Or Lower!")]
        public string BloodType { get; set; } = null!;
        public string? Note { get; set; }
    }
}
