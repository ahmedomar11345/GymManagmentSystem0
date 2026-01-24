using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.SessionViewModel
{
    public class UpdateSessionViewModel
    {
        // Description
        [Required(ErrorMessage = "Description is required")]
        [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500")]
        public string Description { get; set; } = null!;

        // Start Date & Time
        [Required(ErrorMessage = "Start date is required")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        // End Date & Time
        [Required(ErrorMessage = "End date is required")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        // Trainer ID
        [Required(ErrorMessage = "Trainer is required")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
    }
}
