using GymManagmentDAL.Repositories.Interfaces;
using GymManagmentBLL.ViewModels.StoreViewModel;
using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IStoreService
    {
        // ── Categories ──────────────────────────────────────────
        Task<IEnumerable<StoreCategoryViewModel>> GetAllCategoriesAsync();
        Task<StoreCategoryViewModel?> GetCategoryByIdAsync(int id);
        Task CreateCategoryAsync(StoreCategoryViewModel model);
        Task UpdateCategoryAsync(StoreCategoryViewModel model);
        Task DeleteCategoryAsync(int id);

        // ── Products ────────────────────────────────────────────
        Task<PagedResult<StoreProductViewModel>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 12, int? categoryId = null, string? search = null);
        Task<StoreProductViewModel?> GetProductByIdAsync(int id);
        Task<StoreProductViewModel?> GetProductByBarcodeAsync(string barcode);
        Task CreateProductAsync(StoreProductViewModel model);
        Task UpdateProductAsync(StoreProductViewModel model);
        Task DeleteProductAsync(int id);
        Task<bool> DeleteProductImageAsync(int imageId);

        // ── Variants ──────────────────────────────────────────
        Task<IEnumerable<StoreProductVariantViewModel>> GetVariantsByProductIdAsync(int productId);
        Task CreateVariantAsync(StoreProductVariantViewModel model, int productId);
        Task UpdateVariantAsync(StoreProductVariantViewModel model);
        Task DeleteVariantAsync(int id);

        // ── Sales ───────────────────────────────────────────────
        Task<string?> ValidateSaleAsync(CreateSaleViewModel model);
        Task<int> CreateSaleAsync(CreateSaleViewModel model);
        Task<PagedResult<SaleDetailsViewModel>> GetSalesHistoryAsync(int pageNumber = 1, int pageSize = 10, System.DateTime? from = null, System.DateTime? to = null, string? search = null);
        Task<SaleDetailsViewModel?> GetSaleDetailsAsync(int id);
        Task<bool> ProcessRefundAsync(int saleId, List<(int SaleItemId, int Quantity)> itemsToRefund);

        // ── Suppliers ───────────────────────────────────────────
        Task<IEnumerable<StoreSupplierViewModel>> GetAllSuppliersAsync();
        Task<StoreSupplierViewModel?> GetSupplierByIdAsync(int id);
        Task CreateSupplierAsync(StoreSupplierViewModel model);
        Task UpdateSupplierAsync(StoreSupplierViewModel model);
        Task DeleteSupplierAsync(int id);

        // ── Dashboard & Reporting ───────────────────────────────
        Task<StoreDashboardStatsViewModel> GetDashboardStatsAsync();
        Task<StoreReportViewModel> GetStoreReportAsync(System.DateTime from, System.DateTime to);

        // ── Stock Adjustments ───────────────────────────────────
        Task<IEnumerable<StockAdjustmentViewModel>> GetStockAdjustmentsAsync(int? productId = null);
        Task CreateStockAdjustmentAsync(StockAdjustmentViewModel model);
        Task QuickStockAddAsync(int productId, int amount);

        // ── Expiry & Stock Alerts ──────────────────────────────
        Task<IEnumerable<StoreProductViewModel>> GetExpiringProductsAsync(int withinDays = 30);
        Task<IEnumerable<StoreProductViewModel>> GetLowStockProductsAsync();

        // ── Bulk Operations ──────────────────────────────────
        Task<BulkImportResultViewModel> BulkImportProductsAsync(BulkImportRequestViewModel model);

        // ── Marketing & Loyalty ────────────────────────────────────────────────
        Task<decimal> GetMemberPointsAsync(int memberId);
        Task<Coupon?> ValidateCouponAsync(string code, decimal currentTotal);
        Task<decimal> CalculateOrderPointsAsync(decimal netTotal);

        // ── Purchases ───────────────────────────────────────────
        Task<IEnumerable<StorePurchaseHistoryViewModel>> GetPurchaseHistoryAsync(int? productId = null, int? supplierId = null, DateTime? from = null, DateTime? to = null);
        Task CreatePurchaseAsync(StorePurchaseViewModel model);
    }
}
