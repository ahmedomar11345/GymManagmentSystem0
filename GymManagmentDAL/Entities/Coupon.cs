using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public enum DiscountType
    {
        Percentage = 0,
        FixedAmount = 1
    }

    public class Coupon : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = null!;

        public DiscountType DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public int UsageLimit { get; set; } = 100;

        public int UsedCount { get; set; } = 0;

        /// <summary>Minimum order amount required to use this coupon</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumAmount { get; set; }
    }
}
