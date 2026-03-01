using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class Sale : BaseEntity
    {
        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBeforeDiscount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDiscount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetTotal { get; set; } = 0;

        /// <summary>Total profit from this sale (tracked at time of sale)</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalProfit { get; set; } = 0;

        /// <summary>Walk-in customer name (optional)</summary>
        [MaxLength(200)]
        public string? CustomerName { get; set; }

        /// <summary>Link to a registered member (optional)</summary>
        public int? MemberId { get; set; }
        public Member? Member { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [MaxLength(500)]
        public string? Notes { get; set; }

        /// <summary>Snapshot of member name at time of sale</summary>
        [MaxLength(200)]
        public string? MemberNameSnapshot { get; set; }

        /// <summary>Snapshot of member status at time of sale (Active/Expired/etc)</summary>
        [MaxLength(50)]
        public string? MemberStatusSnapshot { get; set; }

        /// <summary>Status of the sale (Completed, PartiallyRefunded, Voided)</summary>
        public SaleStatus Status { get; set; } = SaleStatus.Completed;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PointsEarned { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PointsRedeemedValue { get; set; } = 0;

        [MaxLength(100)]
        public string? CouponCode { get; set; }

        [MaxLength(50)]
        public string? InvoiceNumber { get; set; }

        [MaxLength(200)]
        public string? CashierName { get; set; }

        [MaxLength(200)]
        public string? CashierEmail { get; set; }

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
