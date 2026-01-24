using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [Required(ErrorMessage = "Email Is Required!")]
        [EmailAddress(ErrorMessage = "Invalid Email Format!")] // Partially visible
        [DataType(DataType.EmailAddress)]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Email Must Be Between 5 and 100 Chars!")]
        public string Email { get; set; } = null!;

        // Phone Validation (Visible)
        [Required(ErrorMessage = "Phone Is Required!")]
        [Phone(ErrorMessage = "Invalid Phone Number!")]
        [RegularExpression(@"^01(0|1|2|5)\d{8}$", ErrorMessage = "Phone must be a valid Egyptian number (01XXXXXXXXX)")]


        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;

        // Building Number Validation (Visible)
        [Required(ErrorMessage = "Building Number Is Required!")]
        [Range(minimum: 1, maximum: 9000, ErrorMessage = "Building Number Must Be Between 1 and 9000!")]
        public int BuildingNumber { get; set; } = default;

        // Street Validation (Visible)
        [Required(ErrorMessage = "Street Is Required!")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "Street Must Be Between 2 and 30 Chars!")]
        public string Street { get; set; } = null!;

        // City Validation (Visible)
        [Required(ErrorMessage = "City Is Required!")]
        [StringLength(maximumLength: 30, MinimumLength = 2, ErrorMessage = "City Must Be Between 2 and 30 Chars!")]
        [RegularExpression(@"[a-zA-Z\s]*$", ErrorMessage = "City Can Contain Only Letters And Spaces!")]
        public string City { get; set; } = null!;

    }
}
