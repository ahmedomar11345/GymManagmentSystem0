using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class StorePurchase : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }
        public StoreProduct? Product { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? ExpenseId { get; set; }
        public Expense? Expense { get; set; }
    }
}
