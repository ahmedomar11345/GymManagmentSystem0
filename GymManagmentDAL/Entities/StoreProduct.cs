using GymManagmentDAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManagmentDAL.Entities
{
    public class StoreProduct : BaseEntity
    {
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>Cost price for profit calculation</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostPrice { get; set; } = 0;

        /// <summary>Optional sale / discounted price</summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SalePrice { get; set; }

        [Required]
        public int StockQuantity { get; set; } = 0;

        /// <summary>Alert threshold — default 5 units</summary>
        public int LowStockThreshold { get; set; } = 5;

        public string? ImageUrl { get; set; }

        /// <summary>Optional product code (SKU)</summary>
        [MaxLength(100)]
        public string? SKU { get; set; }

        /// <summary>Optional barcode for scanner-assisted selling</summary>
        [MaxLength(100)]
        public string? Barcode { get; set; }

        /// <summary>For supplements and snacks</summary>
        public DateTime? ExpiryDate { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Required]
        public int StoreCategoryId { get; set; }
        public StoreCategory Category { get; set; } = null!;

        public ProductFlavor Flavor { get; set; } = ProductFlavor.None;

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<StoreProductVariant> Variants { get; set; } = new List<StoreProductVariant>();
        public ICollection<StoreProductImage> Images { get; set; } = new List<StoreProductImage>();
    }
}
