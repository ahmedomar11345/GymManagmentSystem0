using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.PlanViewModel
{
    public class CreatePlanViewModel
    {
        [Required(ErrorMessage = "PlanNameRequired")]
        [StringLength(100, ErrorMessage = "PlanNameLength")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "PriceRequired")]
        [Range(0, 50000, ErrorMessage = "Price must be between 0 and 50000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "DurationRequired")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        public int DurationDays { get; set; }

        [Required(ErrorMessage = "DescriptionRequired")]
        [StringLength(500, ErrorMessage = "DescriptionLength")]
        public string Description { get; set; } = string.Empty;
    }
}
