using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class StoreProductVariant : BaseEntity
    {
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public int StoreProductId { get; set; }
        public StoreProduct StoreProduct { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string VariantName { get; set; } = null!; // e.g. "Chocolate Flavor", "Large"

        [MaxLength(100)]
        public string? SKU { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAdjustment { get; set; } = 0; // Sold at BasePrice + Adjustment

        public int StockQuantity { get; set; } = 0;
    }
}
