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

        /// <summary>True if this sale has been voided/cancelled and stock restored.</summary>
        public bool IsVoided { get; set; } = false;

        public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    }
}
