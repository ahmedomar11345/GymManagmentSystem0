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
        public async Task<IActionResult> Index(int? categoryId, string? search)
        {
            var products   = await _storeService.GetAllProductsAsync(categoryId, search);
            var categories = await _storeService.GetAllCategoriesAsync();
            var stats      = await _storeService.GetDashboardStatsAsync();

            ViewBag.Categories  = categories;
            ViewBag.Stats       = stats;
            ViewBag.CategoryId  = categoryId;
            ViewBag.Search      = search;
            return View(products);
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
            var products = await _storeService.GetAllProductsAsync();
            var members  = await _memberService.GetAllMembersAsync();
            var categories = await _storeService.GetAllCategoriesAsync();

            ViewBag.Products = products.Where(p => p.IsAvailable && p.StockQuantity > 0).ToList();
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
            if (ModelState.IsValid)
            {
                model.Items = model.Items.Where(i => i.Quantity > 0).ToList();
                if (!model.Items.Any())
                {
                    TempData["ErrorMessage"] = _localizer["StoreSaleNoItems"].Value;
                    return await ReloadCreateSaleView(model);
                }

                // Server-side: validate stock and discount
                var validationError = await _storeService.ValidateSaleAsync(model);
                if (validationError != null)
                {
                    TempData["ErrorMessage"] = validationError;
                    return await ReloadCreateSaleView(model);
                }

                var saleId = await _storeService.CreateSaleAsync(model);
                TempData["SuccessMessage"] = _localizer["StoreSaleCreated"].Value;
                return RedirectToAction(nameof(SaleDetails), new { id = saleId, print = model.PrintReceipt });
            }
            return await ReloadCreateSaleView(model);
        }

        public async Task<IActionResult> SalesHistory(DateTime? from, DateTime? to, string? search)
        {
            var sales = await _storeService.GetSalesHistoryAsync(from, to, search);
            ViewBag.From   = from?.ToString("yyyy-MM-dd");
            ViewBag.To     = to?.ToString("yyyy-MM-dd");
            ViewBag.Search = search;
            return View(sales);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoidSale(int id)
        {
            var result = await _storeService.VoidSaleAsync(id);
            if (result)
                TempData["SuccessMessage"] = _localizer["SaleVoided"].Value;
            else
                TempData["ErrorMessage"] = _localizer["SaleVoidFailed"].Value;
            return RedirectToAction(nameof(SalesHistory));
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
        public async Task<IActionResult> StockAdjustments()
        {
            var adjustments = await _storeService.GetStockAdjustmentsAsync();
            var products = await _storeService.GetAllProductsAsync();
            ViewBag.Products = new SelectList(products, "Id", "Name");
            return View(adjustments);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStockAdjustment(StockAdjustmentViewModel model)
        {
            await _storeService.CreateStockAdjustmentAsync(model);
            TempData["SuccessMessage"] = _localizer["StockAdjustmentSaved"].Value;
            return RedirectToAction(nameof(StockAdjustments));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickStockAdd(int productId, int amount)
        {
            if (amount <= 0) return BadRequest();

            var adjustment = new StockAdjustmentViewModel
            {
                ProductId = productId,
                Quantity = amount,
                AdjustmentType = StockAdjustmentType.Addition,
                Reason = "Quick replenishment from Low Stock page",
                AdjustmentDate = DateTime.Now
            };

            await _storeService.CreateStockAdjustmentAsync(adjustment);
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

        // ── AJAX: Barcode Lookup ──────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode)) return BadRequest();
            var product = await _storeService.GetProductByBarcodeAsync(barcode);
            if (product == null) return NotFound();
            return Json(new { id = product.Id, name = product.Name, price = product.EffectivePrice, stock = product.StockQuantity, imageUrl = product.ImageUrl });
        }

        // ── Helpers ───────────────────────────────────────────────────────────────
        private async Task PopulateSelectLists(int? catId = null, int? supId = null)
        {
            var cats = await _storeService.GetAllCategoriesAsync();
            var sups = await _storeService.GetAllSuppliersAsync();
            ViewBag.Categories   = cats;
            ViewBag.CategoryList = new SelectList(cats, "Id", "Name", catId);
            ViewBag.SupplierList = new SelectList(sups, "Id", "Name", supId);
        }

        private SelectList GetPaymentMethodList() =>
            new SelectList(Enum.GetValues<PaymentMethod>().Select(p => new { Value = (int)p, Text = p.ToString() }), "Value", "Text");

        private async Task<IActionResult> ReloadCreateSaleView(CreateSaleViewModel model)
        {
            var products = await _storeService.GetAllProductsAsync();
            var members  = await _memberService.GetAllMembersAsync();
            var categories = await _storeService.GetAllCategoriesAsync();

            ViewBag.Products = products.Where(p => p.IsAvailable && p.StockQuantity > 0).ToList();
            ViewBag.Members  = new SelectList(members, "Id", "Name");
            ViewBag.Categories = categories;
            ViewBag.PaymentMethods = GetPaymentMethodList();
            return View("CreateSale", model);
        }
    }
}
