using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities
{
    public class StoreProductImage : BaseEntity
    {
        [Required]
        public string ImageUrl { get; set; } = null!;

        public int StoreProductId { get; set; }
        public StoreProduct StoreProduct { get; set; } = null!;

        public int DisplayOrder { get; set; } = 0;
    }
}
