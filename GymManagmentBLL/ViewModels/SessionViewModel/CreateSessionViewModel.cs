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
        [Required(ErrorMessage = "DescriptionRequired")]
        [StringLength(maximumLength: 500, MinimumLength = 10, ErrorMessage = "DescriptionLength")]
        public string Description { get; set; } = null!;

        // Capacity
        [Required(ErrorMessage = "CapacityRequired")]
        [Range(minimum: 0, maximum: 25, ErrorMessage = "Capacity must be between 0 and 25.")]
        public int Capacity { get; set; }

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

        // Category ID
        [Required(ErrorMessage = "CategoryRequired")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Recurrence
        [Display(Name = "Recurrence")]
        public GymManagmentDAL.Entities.Enums.RecurrenceType RecurrenceType { get; set; } = GymManagmentDAL.Entities.Enums.RecurrenceType.None;

        [Range(1, 52, ErrorMessage = "Recurrence count must be between 1 and 52.")]
        [Display(Name = "Repeat For")]
        public int RecurrenceCount { get; set; } = 1;
    }
}
