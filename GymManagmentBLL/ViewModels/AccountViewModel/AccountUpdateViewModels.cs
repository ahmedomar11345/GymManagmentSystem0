using System.ComponentModel.DataAnnotations;

namespace GymManagmentBLL.ViewModels.AccountViewModel
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "LastNameRequired")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "CurrentPasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "NewPasswordRequired")]
        [StringLength(100, ErrorMessage = "PasswordLength", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "PasswordMismatch")]
        public string ConfirmPassword { get; set; } = null!;
    }

    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New Email Address")]
        public string NewEmail { get; set; } = null!;
    }

    public class VerifyOtpViewModel
    {
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        [Display(Name = "OTP Code")]
        public string OtpCode { get; set; } = null!;
    }
}
