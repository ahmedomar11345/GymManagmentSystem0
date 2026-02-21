using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities
{
    public class Supplier : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? ContactPerson { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public ICollection<StoreProduct> Products { get; set; } = new List<StoreProduct>();
    }
}
