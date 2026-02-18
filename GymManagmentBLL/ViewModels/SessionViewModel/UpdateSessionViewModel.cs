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
        [Required(ErrorMessage = "DescriptionRequired")]
        [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "DescriptionLength")]
        public string Description { get; set; } = null!;

        // Start Date & Time
        [Required(ErrorMessage = "StartDateRequired")]
        [Display(Name = "Start Date & Time")]
        public DateTime StartDate { get; set; }

        // End Date & Time
        [Required(ErrorMessage = "EndDateRequired")]
        [Display(Name = "End Date & Time")]
        public DateTime EndDate { get; set; }

        // Trainer ID
        [Required(ErrorMessage = "TrainerRequired")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
    }
}
