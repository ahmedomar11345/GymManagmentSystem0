using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.StoreViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Entities.Enums;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<StoreService> _logger;
        private static readonly System.Threading.SemaphoreSlim _imageLock = new(1, 1);
        private const int MaxProductImages = 6;

        public StoreService(IUnitOfWork unitOfWork, INotificationService notificationService,
            IWebHostEnvironment env, ILogger<StoreService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _env = env;
            _logger = logger;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private async Task<string?> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext)) return null;

            var dir = Path.Combine(_env.WebRootPath, "uploads", "store");
            Directory.CreateDirectory(dir);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(dir, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/store/{fileName}";
        }

        // ── Categories ────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreCategoryViewModel>> GetAllCategoriesAsync()
        {
            var cats = await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync();
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            return cats.Select(c => new StoreCategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                NameAr = c.NameAr,
                Description = c.Description,
                Icon = c.Icon,
                ProductCount = prods.Count(p => p.StoreCategoryId == c.Id)
            }).ToList();
        }

        public async Task<StoreCategoryViewModel?> GetCategoryByIdAsync(int id)
        {
            var c = await _unitOfWork.GetRepository<StoreCategory>().GetByIdAsync(id);
            if (c == null) return null;
            return new StoreCategoryViewModel { Id = c.Id, Name = c.Name, NameAr = c.NameAr, Description = c.Description, Icon = c.Icon };
        }

        public async Task CreateCategoryAsync(StoreCategoryViewModel model)
        {
            var entity = new StoreCategory { Name = model.Name, NameAr = model.NameAr, Description = model.Description, Icon = model.Icon };
            await _unitOfWork.GetRepository<StoreCategory>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(StoreCategoryViewModel model)
        {
            var entity = await _unitOfWork.GetRepository<StoreCategory>().GetByIdAsync(model.Id);
            if (entity == null) return;
            entity.Name = model.Name;
            entity.NameAr = model.NameAr;
            entity.Description = model.Description;
            entity.Icon = model.Icon;
            _unitOfWork.GetRepository<StoreCategory>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<StoreCategory>().GetByIdAsync(id);
            if (entity == null) return;
            _unitOfWork.GetRepository<StoreCategory>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
        // ── Suppliers ────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreSupplierViewModel>> GetAllSuppliersAsync()
        {
            var suppliers = await _unitOfWork.GetRepository<Supplier>().GetAllAsync();
            var products = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            return suppliers.Select(s => new StoreSupplierViewModel
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Address = s.Address,
                ProductCount = products.Count(p => p.SupplierId == s.Id)
            }).ToList();
        }

        public async Task<StoreSupplierViewModel?> GetSupplierByIdAsync(int id)
        {
            var s = await _unitOfWork.GetRepository<Supplier>().GetByIdAsync(id);
            if (s == null) return null;
            return new StoreSupplierViewModel
            {
                Id = s.Id,
                Name = s.Name,
                ContactPerson = s.ContactPerson,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                Address = s.Address
            };
        }

        public async Task CreateSupplierAsync(StoreSupplierViewModel model)
        {
            var entity = new Supplier
            {
                Name = model.Name,
                ContactPerson = model.ContactPerson,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Address = model.Address
            };
            await _unitOfWork.GetRepository<Supplier>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateSupplierAsync(StoreSupplierViewModel model)
        {
            var entity = await _unitOfWork.GetRepository<Supplier>().GetByIdAsync(model.Id);
            if (entity == null) return;
            entity.Name = model.Name;
            entity.ContactPerson = model.ContactPerson;
            entity.PhoneNumber = model.PhoneNumber;
            entity.Email = model.Email;
            entity.Address = model.Address;
            _unitOfWork.GetRepository<Supplier>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var entity = await _unitOfWork.GetRepository<Supplier>().GetByIdAsync(id);
            if (entity == null) return;
            _unitOfWork.GetRepository<Supplier>().Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Products ──────────────────────────────────────────────────────────
        private async Task<StoreProductViewModel> MapProduct(StoreProduct p)
        {
            var cat = await _unitOfWork.GetRepository<StoreCategory>().GetByIdAsync(p.StoreCategoryId);
            var sup = p.SupplierId.HasValue ? await _unitOfWork.GetRepository<Supplier>().GetByIdAsync(p.SupplierId.Value) : null;

            return new StoreProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                NameAr = p.NameAr,
                Description = p.Description,
                Price = p.Price,
                CostPrice = p.CostPrice,
                SalePrice = p.SalePrice,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                ImageUrl = p.ImageUrl,
                SKU = p.SKU,
                Barcode = p.Barcode,
                ExpiryDate = p.ExpiryDate,
                IsAvailable = p.IsAvailable,
                StoreCategoryId = p.StoreCategoryId,
                CategoryName = cat?.Name,
                CategoryNameAr = cat?.NameAr,
                SupplierId = p.SupplierId,
                SupplierName = sup?.Name,
                GalleryImages = p.Images?.Select(img => new StoreProductImageViewModel
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    DisplayOrder = img.DisplayOrder
                }).OrderBy(img => img.DisplayOrder).ToList() ?? new List<StoreProductImageViewModel>()
            };
        }

        public async Task<IEnumerable<StoreProductViewModel>> GetAllProductsAsync(int? categoryId = null, string? search = null)
        {
            var allProds = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var allCats = await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync();
            var allSups = await _unitOfWork.GetRepository<Supplier>().GetAllAsync();

            var catDict = allCats.ToDictionary(c => c.Id, c => c.Name);
            var supDict = allSups.ToDictionary(s => s.Id, s => s.Name);

            IEnumerable<StoreProduct> result = allProds;
            if (categoryId.HasValue)
                result = result.Where(p => p.StoreCategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(search))
                result = result.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                                        || (p.NameAr != null && p.NameAr.Contains(search))
                                        || (p.Barcode != null && p.Barcode.Contains(search))
                                        || (p.SKU != null && p.SKU.Contains(search)));

            return result.Select(p =>
            {
                var vm = new StoreProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    NameAr = p.NameAr,
                    Description = p.Description,
                    Price = p.Price,
                    CostPrice = p.CostPrice,
                    SalePrice = p.SalePrice,
                    StockQuantity = p.StockQuantity,
                    LowStockThreshold = p.LowStockThreshold,
                    ImageUrl = p.ImageUrl,
                    SKU = p.SKU,
                    Barcode = p.Barcode,
                    ExpiryDate = p.ExpiryDate,
                    IsAvailable = p.IsAvailable,
                    StoreCategoryId = p.StoreCategoryId,
                    CategoryName = allCats.FirstOrDefault(c => c.Id == p.StoreCategoryId)?.Name,
                    CategoryNameAr = allCats.FirstOrDefault(c => c.Id == p.StoreCategoryId)?.NameAr,
                    SupplierId = p.SupplierId,
                    SupplierName = p.SupplierId.HasValue && supDict.TryGetValue(p.SupplierId.Value, out var sName) ? sName : null
                };
                return vm;
            }).ToList();
        }

        public async Task<StoreProductViewModel?> GetProductByIdAsync(int id)
        {
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync(p => p.Id == id, p => p.Images);
            var p = prods.FirstOrDefault();
            if (p == null) return null;
            return await MapProduct(p);
        }

        public async Task<StoreProductViewModel?> GetProductByBarcodeAsync(string barcode)
        {
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync(p => p.Barcode == barcode);
            var p = prods.FirstOrDefault();
            if (p == null) return null;
            return await MapProduct(p);
        }

        public async Task CreateProductAsync(StoreProductViewModel model)
        {
            string? imageUrl = model.ImageUrl; // Use provided URL by default if any
            if (model.ImageFile != null)
                imageUrl = await SaveImageAsync(model.ImageFile);

            var entity = new StoreProduct
            {
                Name = model.Name,
                NameAr = model.NameAr,
                Description = model.Description,
                Price = model.Price,
                CostPrice = model.CostPrice,
                SalePrice = model.SalePrice,
                StockQuantity = model.StockQuantity,
                LowStockThreshold = model.LowStockThreshold,
                ImageUrl = imageUrl,
                SKU = model.SKU,
                Barcode = model.Barcode,
                ExpiryDate = model.ExpiryDate,
                IsAvailable = model.IsAvailable,
                StoreCategoryId = model.StoreCategoryId,
                SupplierId = model.SupplierId
            };
            await _unitOfWork.GetRepository<StoreProduct>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // Save additional images with race condition protection
            if (model.AdditionalImages != null && model.AdditionalImages.Any())
            {
                await _imageLock.WaitAsync();
                try
                {
                    int currentCount = !string.IsNullOrEmpty(entity.ImageUrl) ? 1 : 0;
                    int order = 1;
                    foreach (var file in model.AdditionalImages)
                    {
                        if (currentCount >= MaxProductImages) break;
                        
                        var url = await SaveImageAsync(file);
                        if (url != null)
                        {
                            var imgEntity = new StoreProductImage
                            {
                                StoreProductId = entity.Id,
                                ImageUrl = url,
                                DisplayOrder = order++
                            };
                            await _unitOfWork.GetRepository<StoreProductImage>().AddAsync(imgEntity);
                            currentCount++;
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                }
                finally { _imageLock.Release(); }
            }
        }

        public async Task UpdateProductAsync(StoreProductViewModel model)
        {
            var entity = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(model.Id);
            if (entity == null) return;

            if (model.ImageFile != null)
            {
                var newUrl = await SaveImageAsync(model.ImageFile);
                if (newUrl != null) entity.ImageUrl = newUrl;
            }
            else
            {
                // Only update URL if no file is uploaded (might be a new external URL orkeeping existing)
                entity.ImageUrl = model.ImageUrl;
            }

            entity.Name = model.Name;
            entity.NameAr = model.NameAr;
            entity.Description = model.Description;
            entity.Price = model.Price;
            entity.CostPrice = model.CostPrice;
            entity.SalePrice = model.SalePrice;
            entity.StockQuantity = model.StockQuantity;
            entity.LowStockThreshold = model.LowStockThreshold;
            entity.SKU = model.SKU;
            entity.Barcode = model.Barcode;
            entity.ExpiryDate = model.ExpiryDate;
            entity.IsAvailable = model.IsAvailable;
            entity.StoreCategoryId = model.StoreCategoryId;
            entity.SupplierId = model.SupplierId;

            _unitOfWork.GetRepository<StoreProduct>().Update(entity);
            await _unitOfWork.SaveChangesAsync();

            // Handle additional images with race condition protection
            if (model.AdditionalImages != null && model.AdditionalImages.Any())
            {
                await _imageLock.WaitAsync();
                try
                {
                    var existingImages = await _unitOfWork.GetRepository<StoreProductImage>().GetAllAsync(i => i.StoreProductId == entity.Id);
                    int mainImgCount = !string.IsNullOrEmpty(entity.ImageUrl) ? 1 : 0;
                    int currentTotal = mainImgCount + existingImages.Count();
                    
                    if (currentTotal < MaxProductImages)
                    {
                        int order = existingImages.Any() ? existingImages.Max(i => i.DisplayOrder) + 1 : 1;

                        foreach (var file in model.AdditionalImages)
                        {
                            if (currentTotal >= MaxProductImages) break;

                            var url = await SaveImageAsync(file);
                            if (url != null)
                            {
                                var imgEntity = new StoreProductImage
                                {
                                    StoreProductId = entity.Id,
                                    ImageUrl = url,
                                    DisplayOrder = order++
                                };
                                await _unitOfWork.GetRepository<StoreProductImage>().AddAsync(imgEntity);
                                currentTotal++;
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                finally { _imageLock.Release(); }
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var p = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(id);
            if (p != null)
            {
                // Delete related images first
                var imgRepo = _unitOfWork.GetRepository<StoreProductImage>();
                var images = await imgRepo.GetAllAsync(i => i.StoreProductId == id);
                foreach (var img in images)
                {
                    await DeleteProductImageAsync(img.Id);
                }
                
                _unitOfWork.GetRepository<StoreProduct>().Delete(p);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            var repo = _unitOfWork.GetRepository<StoreProductImage>();
            var img = await repo.GetByIdAsync(imageId);
            if (img == null) return false;

            // Delete physical file
            if (!string.IsNullOrEmpty(img.ImageUrl) && img.ImageUrl.StartsWith("/uploads/"))
            {
                var filePath = Path.Combine(_env.WebRootPath, img.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); }
                    catch (Exception ex) { _logger.LogError(ex, "Failed to delete product image file: {Path}", filePath); }
                }
            }

            repo.Delete(img);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // ── Sales ─────────────────────────────────────────────────────────────
        public async Task<string?> ValidateSaleAsync(CreateSaleViewModel model)
        {
            var itemsToProcess = model.Items.Where(i => i.Quantity > 0).ToList();
            decimal totalBeforeDiscount = 0;

            foreach (var item in itemsToProcess)
            {
                var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(item.StoreProductId);
                if (product == null || !product.IsAvailable)
                    return $"Product ID {item.StoreProductId} is not available.";

                if (item.Quantity > product.StockQuantity)
                    return $"insufficient stock for \"{product.Name}\": requested {item.Quantity}, available {product.StockQuantity}.";

                var unitPrice = product.SalePrice ?? product.Price;
                totalBeforeDiscount += unitPrice * item.Quantity;
            }

            if (model.DiscountAmount > totalBeforeDiscount)
                return $"Discount ({model.DiscountAmount:N2}) cannot exceed the order total ({totalBeforeDiscount:N2}).";

            return null; // valid
        }

        public async Task<int> CreateSaleAsync(CreateSaleViewModel model)
        {
            var itemsToProcess = model.Items.Where(i => i.Quantity > 0).ToList();
            if (!itemsToProcess.Any()) return 0;

            var saleItemsData = new List<(StoreProduct Product, int Quantity, decimal UnitPrice, decimal LineTotal, decimal GrossProfit)>();
            decimal totalBeforeDiscount = 0;

            // 1. First pass: Calculate totals and gross profits
            foreach (var item in itemsToProcess)
            {
                var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(item.StoreProductId);
                if (product == null || !product.IsAvailable) continue;

                var unitPrice = product.SalePrice ?? product.Price;
                var lineTotal = unitPrice * item.Quantity;
                var grossProfit = (unitPrice - product.CostPrice) * item.Quantity;

                totalBeforeDiscount += lineTotal;
                saleItemsData.Add((product, item.Quantity, unitPrice, lineTotal, grossProfit));
            }

            if (!saleItemsData.Any()) return 0;

            // 2. Second pass: Create SaleItems with distributed discount
            var saleItems = new List<SaleItem>();
            decimal totalProfit = 0;
            decimal totalDiscount = model.DiscountAmount;

            foreach (var data in saleItemsData)
            {
                // Distribute discount proportionally: (itemPrice / totalBeforeDiscount) * totalDiscount
                decimal itemShareOfDiscount = 0;
                if (totalBeforeDiscount > 0)
                {
                    itemShareOfDiscount = (data.LineTotal / totalBeforeDiscount) * totalDiscount;
                }

                var itemNetProfit = data.GrossProfit - itemShareOfDiscount;
                totalProfit += itemNetProfit;

                // Deduct stock
                data.Product.StockQuantity -= data.Quantity;
                if (data.Product.StockQuantity < 0) data.Product.StockQuantity = 0;
                _unitOfWork.GetRepository<StoreProduct>().Update(data.Product);

                saleItems.Add(new SaleItem
                {
                    StoreProductId = data.Product.Id,
                    Quantity = data.Quantity,
                    UnitPrice = data.UnitPrice,
                    SnapshotCostPrice = data.Product.CostPrice,
                    TotalPrice = data.LineTotal,
                    ItemProfit = itemNetProfit // Store the accounting-correct net profit for this item
                });

                // Low-stock notification
                if (data.Product.StockQuantity <= data.Product.LowStockThreshold)
                {
                    try
                    {
                        await _notificationService.CreateNotificationAsync(
                            $"⚠️ Low Stock: {data.Product.Name}",
                            $"Product \"{data.Product.Name}\" has only {data.Product.StockQuantity} units remaining.",
                            "/Store/Index", null, "warning");
                    }
                    catch (Exception ex) { _logger.LogWarning(ex, "Could not send low-stock notification for product {Id}", data.Product.Id); }
                }
            }

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                TotalBeforeDiscount = totalBeforeDiscount,
                TotalDiscount = totalDiscount,
                NetTotal = totalBeforeDiscount - totalDiscount,
                TotalProfit = totalProfit, // sum of itemNetProfits
                CustomerName = model.CustomerName,
                MemberId = model.MemberId,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes,
                Items = saleItems
            };

            await _unitOfWork.GetRepository<Sale>().AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();
            return sale.Id;
        }

        public async Task<bool> VoidSaleAsync(int id)
        {
            var sale = await _unitOfWork.GetRepository<Sale>().GetByIdAsync(id);
            if (sale == null || sale.IsVoided) return false;

            // Restore stock
            var saleItems = (await _unitOfWork.GetRepository<SaleItem>().GetAllAsync())
                .Where(i => i.SaleId == id).ToList();

            foreach (var item in saleItems)
            {
                var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(item.StoreProductId);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                    _unitOfWork.GetRepository<StoreProduct>().Update(product);
                }
            }

            sale.IsVoided = true;
            _unitOfWork.GetRepository<Sale>().Update(sale);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SaleDetailsViewModel>> GetSalesHistoryAsync(DateTime? from = null, DateTime? to = null, string? search = null)
        {
            var salesRepo = _unitOfWork.GetRepository<Sale>();
            var allSales = await salesRepo.GetAllAsync();
            var items = await _unitOfWork.GetRepository<SaleItem>().GetAllAsync();
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();

            var sales = allSales.Where(s => !s.IsVoided).AsEnumerable();
            if (from.HasValue) sales = sales.Where(s => s.SaleDate >= from.Value);
            if (to.HasValue) sales = sales.Where(s => s.SaleDate <= to.Value.AddDays(1));
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                var memberMatches = members.Where(m => m.Name.ToLower().Contains(q)).Select(m => (int?)m.Id).ToHashSet();
                sales = sales.Where(s =>
                    (s.CustomerName != null && s.CustomerName.ToLower().Contains(q)) ||
                    (s.MemberId.HasValue && memberMatches.Contains(s.MemberId)));
            }

            var prodDict = prods.ToDictionary(p => p.Id);
            var memberDict = members.ToDictionary(m => m.Id);
            var itemsBySale = items.GroupBy(i => i.SaleId).ToDictionary(g => g.Key, g => g.ToList());

            return sales.OrderByDescending(s => s.SaleDate).Select(s =>
            {
                var saleItems = itemsBySale.TryGetValue(s.Id, out var si) ? si : new List<SaleItem>();
                return new SaleDetailsViewModel
                {
                    Id = s.Id,
                    SaleDate = s.SaleDate,
                    TotalBeforeDiscount = s.TotalBeforeDiscount,
                    TotalDiscount = s.TotalDiscount,
                    NetTotal = s.NetTotal,
                    TotalProfit = s.TotalProfit,
                    CustomerName = s.CustomerName,
                    MemberName = s.MemberId.HasValue && memberDict.TryGetValue(s.MemberId.Value, out var m)
                        ? m.Name : null,
                    PaymentMethod = s.PaymentMethod,
                    Notes = s.Notes,
                    Items = saleItems.Select(i => new SaleItemDetailViewModel
                    {
                        ProductName = prodDict.TryGetValue(i.StoreProductId, out var pr) ? pr.Name : "?",
                        ProductImageUrl = pr?.ImageUrl,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        SnapshotCostPrice = i.SnapshotCostPrice,
                        TotalPrice = i.TotalPrice,
                        ItemProfit = i.ItemProfit
                    }).ToList()
                };
            }).ToList();
        }

        public async Task<SaleDetailsViewModel?> GetSaleDetailsAsync(int id)
        {
            var sales = await GetSalesHistoryAsync();
            return sales.FirstOrDefault(s => s.Id == id);
        }

        // ── Dashboard & Reporting ─────────────────────────────────────────────
        public async Task<StoreDashboardStatsViewModel> GetDashboardStatsAsync()
        {
            var sales = await _unitOfWork.GetRepository<Sale>().GetAllAsync();
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var expiryThreshold = today.AddDays(30);

            return new StoreDashboardStatsViewModel
            {
                TodayRevenue = sales.Where(s => s.SaleDate.Date == today).Sum(s => s.NetTotal),
                MonthRevenue = sales.Where(s => s.SaleDate >= monthStart).Sum(s => s.NetTotal),
                MonthProfit = sales.Where(s => s.SaleDate >= monthStart).Sum(s => s.TotalProfit),
                TotalProducts = prods.Count(),
                LowStockCount = prods.Count(p => p.StockQuantity <= p.LowStockThreshold),
                TotalSalesToday = sales.Count(s => s.SaleDate.Date == today),
                ExpiringCount = prods.Count(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value <= expiryThreshold && p.ExpiryDate.Value >= today)
            };
        }

        public async Task<StoreReportViewModel> GetStoreReportAsync(DateTime from, DateTime to)
        {
            var allSales = await GetSalesHistoryAsync(from, to);
            var salesList = allSales.ToList();
            
            var items = salesList.SelectMany(s => s.Items).ToList();
            var topProducts = items.GroupBy(i => i.ProductName)
                .Select(g => new BestSellerViewModel
                {
                    ProductName = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    TotalProfit = g.Sum(x => x.ItemProfit)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToList();

            var totalRevenue = salesList.Sum(s => s.NetTotal);
            var salesCount = salesList.Count;

            // Daily breakdown
            var dailyBreakdown = salesList
                .GroupBy(s => s.SaleDate.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new DailySalesSummary
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(s => s.NetTotal),
                    Profit = g.Sum(s => s.TotalProfit),
                    Discounts = g.Sum(s => s.TotalDiscount)
                }).ToList();

            // Category breakdown — query from SaleItems joined with products
            var saleItemsDb = await _unitOfWork.GetRepository<SaleItem>().GetAllAsync();
            var prodRepo = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var catRepo = await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync();

            var saleIds = salesList.Select(s => s.Id).ToHashSet();
            var prodDict = prodRepo.ToDictionary(p => p.Id);
            var catDict = catRepo.ToDictionary(c => c.Id);

            var categoryBreakdown = saleItemsDb
                .Where(si => saleIds.Contains(si.SaleId))
                .GroupBy(si => {
                    if (prodDict.TryGetValue(si.StoreProductId, out var prod) && catDict.TryGetValue(prod.StoreCategoryId, out var cat))
                        return cat.Name;
                    return "Other";
                })
                .Select(g => new CategoryProfitSummary
                {
                    CategoryName = g.Key,
                    ProductCount = g.Select(x => x.StoreProductId).Distinct().Count(),
                    TotalQtySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    TotalProfit = g.Sum(x => x.ItemProfit)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            return new StoreReportViewModel
            {
                FromDate = from,
                ToDate = to,
                TotalRevenue = totalRevenue,
                TotalProfit = salesList.Sum(s => s.TotalProfit),
                TotalDiscounts = salesList.Sum(s => s.TotalDiscount),
                SalesCount = salesCount,
                AvgOrderValue = salesCount > 0 ? totalRevenue / salesCount : 0,
                TopProducts = topProducts,
                DailyBreakdown = dailyBreakdown,
                CategoryBreakdown = categoryBreakdown
            };
        }

        // ── Stock Adjustments ──────────────────────────────────────────────────
        public async Task<IEnumerable<StockAdjustmentViewModel>> GetStockAdjustmentsAsync(int? productId = null)
        {
            var adjustments = await _unitOfWork.GetRepository<StockAdjustment>().GetAllAsync();
            var products = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var prodDict = products.ToDictionary(p => p.Id, p => p.Name);

            var query = adjustments.AsEnumerable();
            if (productId.HasValue)
                query = query.Where(a => a.ProductId == productId.Value);

            return query.OrderByDescending(a => a.AdjustmentDate)
                .Select(a => new StockAdjustmentViewModel
                {
                    Id = a.Id,
                    ProductId = a.ProductId,
                    ProductName = prodDict.TryGetValue(a.ProductId, out var n) ? n : "?",
                    Quantity = a.Quantity,
                    AdjustmentType = a.AdjustmentType,
                    Reason = a.Reason,
                    AdjustmentDate = a.AdjustmentDate
                }).ToList();
        }

        public async Task CreateStockAdjustmentAsync(StockAdjustmentViewModel model)
        {
            var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(model.ProductId);
            if (product == null) return;

            // Apply adjustment to stock
            product.StockQuantity += model.Quantity;
            if (product.StockQuantity < 0) product.StockQuantity = 0;

            _unitOfWork.GetRepository<StoreProduct>().Update(product);

            var entity = new StockAdjustment
            {
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                AdjustmentType = model.AdjustmentType,
                Reason = model.Reason,
                AdjustmentDate = DateTime.Now
            };

            await _unitOfWork.GetRepository<StockAdjustment>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        // ── Expiry ─────────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreProductViewModel>> GetExpiringProductsAsync(int withinDays = 30)
        {
            var products = await GetAllProductsAsync();
            var threshold = DateTime.Today.AddDays(withinDays);
            return products.Where(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value <= threshold && p.ExpiryDate.Value >= DateTime.Today)
                           .OrderBy(p => p.ExpiryDate);
        }

        public async Task<IEnumerable<StoreProductViewModel>> GetLowStockProductsAsync()
        {
            var products = await GetAllProductsAsync();
            return products.Where(p => p.IsLowStock)
                           .OrderBy(p => p.StockQuantity);
        }
    }
}
