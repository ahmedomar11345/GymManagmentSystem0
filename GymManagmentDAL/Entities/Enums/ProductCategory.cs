using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities.Enums
{
    public enum ProductCategory
    {
        [Display(Name = "Supplements")]
        Supplements = 1,
        [Display(Name = "Snacks")]
        Snacks = 2,
        [Display(Name = "Other")]
        Other = 99
    }
}
