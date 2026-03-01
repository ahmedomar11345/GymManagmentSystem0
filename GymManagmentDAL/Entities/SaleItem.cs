using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class SaleItem : BaseEntity
    {
        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public int StoreProductId { get; set; }
        public StoreProduct Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>Store cost price at time of sale for profit calculation</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SnapshotCostPrice { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ItemProfit { get; set; } = 0;

        /// <summary>Quantity that has been refunded (for partial refunds)</summary>
        public int RefundedQuantity { get; set; } = 0;

        /// <summary>Backlink to specific variant if applicable</summary>
        public int? StoreProductVariantId { get; set; }
        public StoreProductVariant? StoreProductVariant { get; set; }
    }
}
