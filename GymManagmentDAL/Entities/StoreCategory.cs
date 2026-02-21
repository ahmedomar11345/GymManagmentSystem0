using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities
{
    public class StoreCategory : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? NameAr { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>FontAwesome icon class e.g. "fas fa-pills"</summary>
        [MaxLength(100)]
        public string Icon { get; set; } = "fas fa-box";

        public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
    }
}
