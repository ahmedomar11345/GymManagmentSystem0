using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.SessionViewModel
{
    public class CreateSessionViewModel
    {
        [Required(ErrorMessage = "Description is required")]
        [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500")]
        public string Description { get; set; } = null!;

        // Capacity
        [Required(ErrorMessage = "Capacity is required")]
        [Range(minimum: 0, maximum: 25, ErrorMessage = "Capacity must be between 0 and 25")]
        public int Capacity { get; set; }

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

        // Category ID
        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
    }
}
