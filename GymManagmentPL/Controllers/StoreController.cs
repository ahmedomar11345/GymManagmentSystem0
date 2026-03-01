using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.StoreViewModel;
using GymManagmentDAL.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Policy = "AdminOrAbove")]
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IMemberService _memberService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public StoreController(IStoreService storeService, IMemberService memberService,
            IStringLocalizer<SharedResource> localizer)
        {
            _storeService = storeService;
            _memberService = memberService;
            _localizer = localizer;
        }


        // ── Products Index ─────────────────────────────────────────────────────
        public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string? search = null)
        {
            var pagedResult = await _storeService.GetAllProductsAsync(page, 12, categoryId, search);
            var categories  = await _storeService.GetAllCategoriesAsync();
            var stats       = await _storeService.GetDashboardStatsAsync();

            ViewBag.Categories  = categories;
            ViewBag.Stats       = stats;
            ViewBag.CategoryId  = categoryId;
            ViewBag.Search      = search;
            return View(pagedResult);
        }

        // ── Categories ─────────────────────────────────────────────────────────
        public async Task<IActionResult> Categories()
        {
            var cats = await _storeService.GetAllCategoriesAsync();
            return View(cats);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(StoreCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.CreateCategoryAsync(model);
                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = string.Join("<br>", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(StoreCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.UpdateCategoryAsync(model);
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _storeService.DeleteCategoryAsync(id);
            TempData["SuccessMessage"] = _localizer["DeleteSuccess"].Value;
            return RedirectToAction(nameof(Categories));
        }

        // ── Product CRUD ────────────────────────────────────────────────────────
        public async Task<IActionResult> CreateProduct()
        {
            await PopulateSelectLists();
            return View(new StoreProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(StoreProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.CreateProductAsync(model);
                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectLists(model.StoreCategoryId, model.SupplierId);
            return View(model);
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            var model = await _storeService.GetProductByIdAsync(id);
            if (model == null) return NotFound();
            await PopulateSelectLists(model.StoreCategoryId, model.SupplierId);
            return View(model);
        }

        public async Task<IActionResult> ProductDetails(int id)
        {
            var model = await _storeService.GetProductByIdAsync(id);
            if (model == null) return NotFound();
            return PartialView("_ProductDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(StoreProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.UpdateProductAsync(model);
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            await PopulateSelectLists(model.StoreCategoryId, model.SupplierId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _storeService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = _localizer["DeleteSuccess"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductImage(int id)
        {
            var success = await _storeService.DeleteProductImageAsync(id);
            return Json(new { success });
        }

        // ── Suppliers ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Suppliers()
        {
            var suppliers = await _storeService.GetAllSuppliersAsync();
            return View(suppliers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSupplier(StoreSupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.CreateSupplierAsync(model);
                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return RedirectToAction(nameof(Suppliers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSupplier(StoreSupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.UpdateSupplierAsync(model);
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return RedirectToAction(nameof(Suppliers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            await _storeService.DeleteSupplierAsync(id);
            TempData["SuccessMessage"] = _localizer["DeleteSuccess"].Value;
            return RedirectToAction(nameof(Suppliers));
        }
 
        // ── Sales ────────────────────────────────────────────────────────────────
        public async Task<IActionResult> CreateSale(int? productId)
        {
            var pagedProds = await _storeService.GetAllProductsAsync(1, 1000); // Get many for the modal
            var members  = await _memberService.GetAllMembersAsync();
            var categories = await _storeService.GetAllCategoriesAsync();

            ViewBag.Products = pagedProds.Items.Where(p => p.IsAvailable && (p.StockQuantity > 0 || p.Variants.Any())).ToList();
            ViewBag.Members  = new SelectList(members, "Id", "Name");
            ViewBag.Categories = categories;
            ViewBag.PaymentMethods = GetPaymentMethodList();
            ViewBag.SelectedProductId = productId;
            return View(new CreateSaleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSale(CreateSaleViewModel model)
        {
            // Always filter to only items with quantity > 0 before anything else
            model.Items = model.Items?.Where(i => i.Quantity > 0).ToList() ?? new();

            if (!model.Items.Any())
            {
                TempData["ErrorMessage"] = _localizer["StoreSaleNoItems"].Value;
                return await ReloadCreateSaleView(model);
            }

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return await ReloadCreateSaleView(model);
            }

            // Server-side: validate stock and discount
            var validationError = await _storeService.ValidateSaleAsync(model);
            if (validationError != null)
            {
                TempData["ErrorMessage"] = validationError;
                return await ReloadCreateSaleView(model);
            }

            try
            {
                // Capture current user as cashier
                model.CashierName = User.Identity?.Name ?? "Admin";
                model.CashierEmail = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value 
                                     ?? User.Identity?.Name 
                                     ?? "system@gym.com";

                var saleId = await _storeService.CreateSaleAsync(model);
                if (saleId > 0)
                {
                    TempData["SuccessMessage"] = _localizer["StoreSaleCreated"].Value;
                    return RedirectToAction(nameof(SaleDetails), new { id = saleId, print = model.PrintReceipt });
                }

                TempData["ErrorMessage"] = _localizer["StoreSaleFailed"].Value;
                return await ReloadCreateSaleView(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return await ReloadCreateSaleView(model);
            }
        }

        public async Task<IActionResult> SalesHistory(int page = 1, DateTime? from = null, DateTime? to = null, string? search = null)
        {
            var pagedSales = await _storeService.GetSalesHistoryAsync(page, 10, from, to, search);
            ViewBag.From   = from?.ToString("yyyy-MM-dd");
            ViewBag.To     = to?.ToString("yyyy-MM-dd");
            ViewBag.Search = search;
            return View(pagedSales);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoidSale(int id)
        {
            var sale = await _storeService.GetSaleDetailsAsync(id);
            if (sale == null) return NotFound();

            // Treat Void as a full refund of all items
            var itemsToRefund = sale.Items.Select(i => (i.Id, i.Quantity)).ToList();
            var result = await _storeService.ProcessRefundAsync(id, itemsToRefund);

            if (result)
                TempData["SuccessMessage"] = _localizer["SaleVoided"].Value;
            else
                TempData["ErrorMessage"] = _localizer["SaleVoidFailed"].Value;

            return RedirectToAction(nameof(SalesHistory));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessRefund(int saleId, int saleItemId, int quantity)
        {
            var itemsToRefund = new List<(int, int)> { (saleItemId, quantity) };
            var result = await _storeService.ProcessRefundAsync(saleId, itemsToRefund);
            
            if (result)
                TempData["SuccessMessage"] = _localizer["RefundProcessed"].Value;
            else
                TempData["ErrorMessage"] = _localizer["RefundFailed"].Value;

            return RedirectToAction(nameof(SaleDetails), new { id = saleId });
        }

        public async Task<IActionResult> SaleDetails(int id)
        {
            var sale = await _storeService.GetSaleDetailsAsync(id);
            if (sale == null) return NotFound();
            return View(sale);
        }

        // ── Reports ─────────────────────────────────────────────────────────────
        public async Task<IActionResult> FinancialReports(DateTime? from, DateTime? to)
        {
            var fromDate = from ?? DateTime.Today.AddMonths(-1);
            var toDate = to ?? DateTime.Today;

            var report = await _storeService.GetStoreReportAsync(fromDate, toDate);
            return View(report);
        }

        // ── Stock Adjustments ─────────────────────────────────────────────────────
        public async Task<IActionResult> StockAdjustments(int? productId)
        {
            var adjustments = await _storeService.GetStockAdjustmentsAsync(productId);
            var products = await _storeService.GetAllProductsAsync(1, 1000);
            ViewBag.Products = new SelectList(products.Items, "Id", "Name", productId);
            ViewBag.SelectedProductId = productId;
            return View(adjustments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStockAdjustment(StockAdjustmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.CreateStockAdjustmentAsync(model);
                TempData["SuccessMessage"] = _localizer["StockAdjustmentSaved"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return RedirectToAction(nameof(StockAdjustments), new { productId = model.ProductId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickStockAdd(int productId, int amount)
        {
            if (amount <= 0) return BadRequest();
            await _storeService.QuickStockAddAsync(productId, amount);
            TempData["SuccessMessage"] = _localizer["StockAdjustmentSaved"].Value;
            return RedirectToAction(nameof(LowStockProducts));
        }

        // ── Expiring Products ─────────────────────────────────────────────────────
        public async Task<IActionResult> ExpiringProducts()
        {
            var products = await _storeService.GetExpiringProductsAsync(30);
            return View(products);
        }

        public async Task<IActionResult> LowStockProducts()
        {
            var products = await _storeService.GetLowStockProductsAsync();
            return View(products);
        }

        // ── Bulk Import ────────────────────────────────────────────────────────
        public IActionResult BulkImport() => View(new BulkImportRequestViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkImport(BulkImportRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    var result = await _storeService.BulkImportProductsAsync(model);
                    return View("BulkImportResult", result);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }
            return View(model);
        }

        // ── Variants ───────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetVariants(int productId)
        {
            var variants = await _storeService.GetVariantsByProductIdAsync(productId);
            return Json(variants);
        }

        // ── Marketing & Loyalty ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetMemberPoints(int memberId)
        {
            var points = await _storeService.GetMemberPointsAsync(memberId);
            return Json(new { points });
        }

        [HttpGet]
        public async Task<IActionResult> ValidateCoupon(string code, decimal total)
        {
            var coupon = await _storeService.ValidateCouponAsync(code, total);
            if (coupon == null) return Json(new { valid = false });

            return Json(new { 
                valid = true, 
                discountType = (int)coupon.DiscountType, 
                value = coupon.Value,
                code = coupon.Code
            });
        }

        // ── AJAX: Barcode Lookup ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return BadRequest();
            var product = await _storeService.GetProductByBarcodeAsync(barcode);
            if (product == null) return NotFound();
            return Json(new { id = product.Id, name = product.Name, price = product.EffectivePrice, stock = product.StockQuantity, imageUrl = product.ImageUrl });
        }
        
        // ── Purchases ──────────────────────────────────────────────────────────
        public async Task<IActionResult> Purchases(int? productId = null, int? supplierId = null, DateTime? from = null, DateTime? to = null)
        {
            var history = await _storeService.GetPurchaseHistoryAsync(productId, supplierId, from, to);
            
            ViewBag.Products = new SelectList((await _storeService.GetAllProductsAsync(1, 1000)).Items, "Id", "Name", productId);
            ViewBag.Suppliers = new SelectList(await _storeService.GetAllSuppliersAsync(), "Id", "Name", supplierId);
            ViewBag.From = from?.ToString("yyyy-MM-dd");
            ViewBag.To = to?.ToString("yyyy-MM-dd");

            return View(history);
        }

        public async Task<IActionResult> CreatePurchase(int? productId = null)
        {
            await PopulateSelectLists(null, null, productId);
            return View(new StorePurchaseViewModel { ProductId = productId ?? 0, PurchaseDate = DateTime.Now, CreateExpenseRecord = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePurchase(StorePurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _storeService.CreatePurchaseAsync(model);
                TempData["SuccessMessage"] = _localizer["PurchaseRecordedSuccess"].Value;
                return RedirectToAction(nameof(Purchases));
            }

            await PopulateSelectLists(null, null, model.ProductId);
            return View(model);
        }

        // ── Helpers ───────────────────────────────────────────────────────────────
        private async Task PopulateSelectLists(int? catId = null, int? supId = null, int? prodId = null)
        {
            var cats = await _storeService.GetAllCategoriesAsync();
            var sups = await _storeService.GetAllSuppliersAsync();
            var prods = await _storeService.GetAllProductsAsync(1, 1000);
            
            ViewBag.Categories   = cats;
            ViewBag.CategoryList = new SelectList(cats, "Id", "Name", catId);
            ViewBag.SupplierList = new SelectList(sups, "Id", "Name", supId);
            ViewBag.ProductList  = new SelectList(prods.Items, "Id", "Name", prodId);
        }

        private SelectList GetPaymentMethodList() =>
            new SelectList(Enum.GetValues<PaymentMethod>().Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text");

        private async Task<IActionResult> ReloadCreateSaleView(CreateSaleViewModel model)
        {
            var products = await _storeService.GetAllProductsAsync(1, 1000);
            var members  = await _memberService.GetAllMembersAsync();
            var categories = await _storeService.GetAllCategoriesAsync();

            ViewBag.Products = products.Items.Where(p => p.IsAvailable && (p.StockQuantity > 0 || p.Variants.Any())).ToList();
            ViewBag.Members  = new SelectList(members, "Id", "Name");
            ViewBag.Categories = categories;
            ViewBag.PaymentMethods = GetPaymentMethodList();
            return View("CreateSale", model);
        }
    }
}
