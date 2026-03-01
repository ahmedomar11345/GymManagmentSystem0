using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities.Enums
{
    public enum ProductFlavor
    {
        [Display(Name = "None")]
        None = 0,
        [Display(Name = "Chocolate")]
        Chocolate,
        [Display(Name = "Vanilla")]
        Vanilla,
        [Display(Name = "Cookies & Cream")]
        CookiesAndCream,
        [Display(Name = "Strawberry")]
        Strawberry,
        [Display(Name = "Banana")]
        Banana,
        [Display(Name = "Salted Caramel")]
        SaltedCaramel,
        [Display(Name = "Blue Raspberry")]
        BlueRaspberry,
        [Display(Name = "Fruit Punch")]
        FruitPunch,
        [Display(Name = "Watermelon")]
        Watermelon,
        [Display(Name = "Mango")]
        Mango,
        [Display(Name = "Peanut Butter")]
        PeanutButter,
        [Display(Name = "Caramel")]
        Caramel
    }
}
