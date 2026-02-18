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
        [Required(ErrorMessage = "HeightRequired")]
        [Range(minimum: 30, maximum: 220, ErrorMessage = "Height must be between 30 and 220 cm.")]
        public decimal Height { get; set; } = default;

        [Required(ErrorMessage = "WeightRequired")]
        [Range(minimum: 30, maximum: 200, ErrorMessage = "Weight must be between 30 and 200 kg.")]
        public decimal Weight { get; set; } = default;

        [Required(ErrorMessage = "BloodTypeRequired")]
        [StringLength(maximumLength: 3, MinimumLength= 1, ErrorMessage = "BloodTypeLength")]
        public string BloodType { get; set; } = null!;
        public string? Note { get; set; }
    }
}
