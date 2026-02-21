using GymManagmentDAL.Entities.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymManagmentBLL.ViewModels.StoreViewModel
{
    // ── Supplier ─────────────────────────────────────────────
    public class StoreSupplierViewModel
    {
        public int Id { get; set; }

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

        public int ProductCount { get; set; }
    }

    // ── Category ─────────────────────────────────────────────
    public class StoreCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(100)]
        public string? NameAr { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string Icon { get; set; } = "fas fa-box";

        public int ProductCount { get; set; }
    }

    // ── Product ──────────────────────────────────────────────
    public class StoreProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CostPrice { get; set; }

        public decimal? SalePrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public int LowStockThreshold { get; set; } = 5;

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

        [MaxLength(100)]
        public string? SKU { get; set; }

        [MaxLength(100)]
        public string? Barcode { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Required]
        public int StoreCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryNameAr { get; set; }

        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }

        // Computed helpers
        public decimal EffectivePrice => SalePrice ?? Price;
        public decimal ExpectedProfitPerUnit => EffectivePrice - CostPrice;
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Today;
        public bool IsOnSale => SalePrice.HasValue && SalePrice.Value < Price;

        public List<StoreProductImageViewModel> GalleryImages { get; set; } = new();
        public List<IFormFile>? AdditionalImages { get; set; }
    }

    public class StoreProductImageViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    // ── Sale ─────────────────────────────────────────────────
    public class CreateSaleViewModel
    {
        public string? CustomerName { get; set; }
        public int? MemberId { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        [Range(0, double.MaxValue)]
        public decimal DiscountAmount { get; set; } = 0;

        public string? Notes { get; set; }

        [Required]
        public List<SaleItemInputViewModel> Items { get; set; } = new();

        public bool PrintReceipt { get; set; } = true;
    }

    public class SaleItemInputViewModel
    {
        public int StoreProductId { get; set; }
        public int Quantity { get; set; }
    }

    // ── Sale Details (read-only) ──────────────────────────────
    public class SaleDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal TotalBeforeDiscount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetTotal { get; set; }
        public decimal TotalProfit { get; set; }
        public string? CustomerName { get; set; }
        public string? MemberName { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Notes { get; set; }
        public List<SaleItemDetailViewModel> Items { get; set; } = new();
    }

    public class SaleItemDetailViewModel
    {
        public string ProductName { get; set; } = null!;
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SnapshotCostPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ItemProfit { get; set; }
    }

    // ── Dashboard stats ───────────────────────────────────────
    public class StoreDashboardStatsViewModel
    {
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal MonthProfit { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int TotalSalesToday { get; set; }
        public int ExpiringCount { get; set; }
    }

    // ── Reporting ─────────────────────────────────────────────
    public class BestSellerViewModel
    {
        public string ProductName { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class CategoryProfitSummary
    {
        public string CategoryName { get; set; } = null!;
        public int ProductCount { get; set; }
        public int TotalQtySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
    }

    public class StoreReportViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalDiscounts { get; set; }
        public int SalesCount { get; set; }
        public decimal AvgOrderValue { get; set; }
        public List<BestSellerViewModel> TopProducts { get; set; } = new();
        public List<DailySalesSummary> DailyBreakdown { get; set; } = new();
        public List<CategoryProfitSummary> CategoryBreakdown { get; set; } = new();
    }

    public class DailySalesSummary
    {
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal Profit { get; set; }
        public decimal Discounts { get; set; }
    }

    // ── Stock Adjustment ─────────────────────────────────────
    public class StockAdjustmentViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public GymManagmentDAL.Entities.Enums.StockAdjustmentType AdjustmentType { get; set; }
        public string? Reason { get; set; }
        public DateTime AdjustmentDate { get; set; }
    }

}
