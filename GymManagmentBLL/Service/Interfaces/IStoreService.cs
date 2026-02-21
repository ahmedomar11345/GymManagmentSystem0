using GymManagmentBLL.ViewModels.StoreViewModel;
using GymManagmentDAL.Entities;
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
        Task<IEnumerable<StoreProductViewModel>> GetAllProductsAsync(int? categoryId = null, string? search = null);
        Task<StoreProductViewModel?> GetProductByIdAsync(int id);
        Task<StoreProductViewModel?> GetProductByBarcodeAsync(string barcode);
        Task CreateProductAsync(StoreProductViewModel model);
        Task UpdateProductAsync(StoreProductViewModel model);
        Task DeleteProductAsync(int id);
        Task<bool> DeleteProductImageAsync(int imageId);

        // ── Sales ───────────────────────────────────────────────
        Task<string?> ValidateSaleAsync(CreateSaleViewModel model);
        Task<int> CreateSaleAsync(CreateSaleViewModel model);
        Task<IEnumerable<SaleDetailsViewModel>> GetSalesHistoryAsync(System.DateTime? from = null, System.DateTime? to = null, string? search = null);
        Task<SaleDetailsViewModel?> GetSaleDetailsAsync(int id);
        Task<bool> VoidSaleAsync(int id);

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

        // ── Expiry & Stock Alerts ──────────────────────────────
        Task<IEnumerable<StoreProductViewModel>> GetExpiringProductsAsync(int withinDays = 30);
        Task<IEnumerable<StoreProductViewModel>> GetLowStockProductsAsync();
    }
}
