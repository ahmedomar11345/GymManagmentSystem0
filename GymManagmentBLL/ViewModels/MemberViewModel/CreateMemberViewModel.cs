using GymManagmentDAL.Entities.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    public class CreateMemberViewModel
    {
        [Display(Name = "Profile Photo")]
        public IFormFile? PhotoFile { get; set; }

        [Required(ErrorMessage = "NameRequired")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "NameLength")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "EmailLength")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "PhoneRequired")]
        [Phone(ErrorMessage = "InvalidPhone")]
        [RegularExpression(@"^01(0|1|2|5)\d{8}$", ErrorMessage = "EgyptianPhone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "DateOfBirthRequired")]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; } = default;

        [Required(ErrorMessage = "GenderRequired")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "BuildingNumberRequired")]
        [Range(minimum: 1, maximum: 9000, ErrorMessage = "Building number must be between 1 and 9000.")]
        public int BuildingNumber { get; set; } = default;

        [Required(ErrorMessage = "StreetRequired")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "StreetLength")]
        public string Street { get; set; } = null!;

        [Required(ErrorMessage = "CityRequired")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "CityLength")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "CountryRequired")]
        public string Country { get; set; } = "Egypt";

        [Required(ErrorMessage = "HealthRecordRequired")]
        public HealthRecordViewModel HealthRecord { get; set; } = null!;

        [Display(Name = "Gym Membership Plan (Optional)")]
        public int? PlanId { get; set; }

        public IEnumerable<GymManagmentBLL.ViewModels.PlanViewModel.PlanViewModel>? Plans { get; set; }

        public bool SendInArabic { get; set; } = true;
    }
}
