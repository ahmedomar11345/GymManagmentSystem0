using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagmentDAL.Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace GymManagmentBLL.ViewModels.MemberViewModel
{
    public class MemberToUpdateViewModel
    {
        public string Name { get; set; } = null!; // Property Name is visible

        public string? Photo { get; set; }  // Property Photo is visible

        [Display(Name = "Profile Photo")]
        public IFormFile? PhotoFile { get; set; }

        // Email Validation (Partially visible/standard)
        [Required(ErrorMessage = "EmailRequired")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        [DataType(DataType.EmailAddress)]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "EmailLength")]
        public string Email { get; set; } = null!;

        // Phone Validation (Visible)
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

        // Building Number Validation (Visible)
        [Required(ErrorMessage = "BuildingNumberRequired")]
        [Range(minimum: 1, maximum: 9000, ErrorMessage = "Building number must be between 1 and 9000.")]
        public int BuildingNumber { get; set; } = default;

        // Street Validation (Visible)
        [Required(ErrorMessage = "StreetRequired")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "StreetLength")]
        public string Street { get; set; } = null!;

        // City Validation (Visible)
        [Required(ErrorMessage = "CityRequired")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "CityLength")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "CountryRequired")]
        public string Country { get; set; } = "Egypt";

        public HealthRecordViewModel HealthRecord { get; set; } = new HealthRecordViewModel();

    }
}
