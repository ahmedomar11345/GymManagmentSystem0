using GymManagmentDAL.Entities.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class StockAdjustment : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }
        public StoreProduct Product { get; set; } = null!;

        /// <summary>Positive = stock in, Negative = stock out</summary>
        [Required]
        public int Quantity { get; set; }

        [Required]
        public StockAdjustmentType AdjustmentType { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        [Required]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
    }
}
