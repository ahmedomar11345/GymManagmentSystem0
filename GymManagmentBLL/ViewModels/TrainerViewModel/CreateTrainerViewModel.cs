using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.TrainerViewModel
{
	public class CreateTrainerViewModel
	{
		[Required(ErrorMessage = "NameRequired")]
		public string Name { get; set; } = null!;

		[Required(ErrorMessage = "EmailRequired")]
		[EmailAddress(ErrorMessage = "InvalidEmail")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "PhoneRequired")]
		[Phone(ErrorMessage = "InvalidPhone")]
		[RegularExpression(@"^01(0|1|2|5)\d{8}$", ErrorMessage = "EgyptianPhone")]
		public string Phone { get; set; } = null!;

		[Required(ErrorMessage = "DateOfBirthRequired")]
		[DataType(DataType.Date)]
		public DateOnly DateOfBirth { get; set; }

		[Required(ErrorMessage = "GenderRequired")]
		public Gender Gender { get; set; }

		[Required(ErrorMessage = "BuildingNumberRequired")]
		[Range(1, int.MaxValue, ErrorMessage = "Building number must be at least 1.")]
		public int BuildingNumber { get; set; }

		[Required(ErrorMessage = "CityRequired")]
		[StringLength(100, MinimumLength = 2, ErrorMessage = "CityLength")]
		public string City { get; set; } = null!;

		[Required(ErrorMessage = "StreetRequired")]
		[StringLength(150, MinimumLength = 2, ErrorMessage = "StreetLength")]
		public string Street { get; set; } = null!;

		[Required(ErrorMessage = "CountryRequired")]
		public string Country { get; set; } = "Egypt";

		[Required(ErrorMessage = "SpecialtyRequired")]
		public int SpecialtyId { get; set; }

		[Required(ErrorMessage = "ShiftStartRequired")]
		public TimeSpan ShiftStart { get; set; } = new TimeSpan(9, 0, 0);

		[Required(ErrorMessage = "ShiftEndRequired")]
		public TimeSpan ShiftEnd { get; set; } = new TimeSpan(17, 0, 0);

		public bool SendInArabic { get; set; } = true;
	}
}
