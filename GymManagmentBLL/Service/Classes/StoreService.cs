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
using System.Linq.Expressions;
using ClosedXML.Excel;

namespace GymManagmentBLL.Service.Classes
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IBroadcastService _broadcastService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<StoreService> _logger;
        private static readonly System.Threading.SemaphoreSlim _imageLock = new(1, 1);
        private const int MaxProductImages = 6;

        public StoreService(IUnitOfWork unitOfWork, INotificationService notificationService,
            IBroadcastService broadcastService, IWebHostEnvironment env, ILogger<StoreService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _broadcastService = broadcastService;
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
                Flavor = p.Flavor, 
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

        public async Task<PagedResult<StoreProductViewModel>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 12, int? categoryId = null, string? search = null)
        {
            Expression<Func<StoreProduct, bool>>? filter = p => true;
            if (categoryId.HasValue)
            {
                var oldFilter = filter;
                filter = p => p.StoreCategoryId == categoryId.Value;
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                var oldFilter = filter;
                filter = p => (p.Name.ToLower().Contains(q) || (p.NameAr != null && p.NameAr.ToLower().Contains(q)) || (p.Barcode != null && p.Barcode.ToLower().Contains(q)) || (p.SKU != null && p.SKU.ToLower().Contains(q))) && (categoryId == null || p.StoreCategoryId == categoryId);
            }

            var pagedProds = await _unitOfWork.GetRepository<StoreProduct>().GetPagedAsync(
                pageNumber, pageSize, filter, 
                query => query.OrderByDescending(p => p.Id));

            var allCats = await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync();
            var allSups = await _unitOfWork.GetRepository<Supplier>().GetAllAsync();
            var catDict = allCats.ToDictionary(c => c.Id);
            var supDict = allSups.ToDictionary(s => s.Id);

            return new PagedResult<StoreProductViewModel>
            {
                PageNumber = pagedProds.PageNumber,
                PageSize = pagedProds.PageSize,
                TotalCount = pagedProds.TotalCount,
                Items = pagedProds.Items.Select(p => {
                    string? sName = null;
                    if (p.SupplierId.HasValue && supDict.TryGetValue(p.SupplierId.Value, out var sup)) sName = sup.Name;
                    
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
                        Flavor = p.Flavor,
                        StoreCategoryId = p.StoreCategoryId,
                        CategoryName = catDict.TryGetValue(p.StoreCategoryId, out var c) ? c.Name : null,
                        CategoryNameAr = c?.NameAr,
                        SupplierId = p.SupplierId,
                        SupplierName = sName
                    };
                }).ToList()
            };
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
            string? imageUrl = model.ImageUrl; 
            if (model.ImageFile != null)
                imageUrl = await SaveImageAsync(model.ImageFile);

            int finalCategoryId = model.StoreCategoryId;

            // Handle dynamic category creation
            if (!string.IsNullOrWhiteSpace(model.NewCategoryName))
            {
                var existingCat = (await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync(c => c.Name.ToLower() == model.NewCategoryName.Trim().ToLower())).FirstOrDefault();
                if (existingCat != null)
                {
                    finalCategoryId = existingCat.Id;
                }
                else
                {
                    var newCat = new StoreCategory { Name = model.NewCategoryName.Trim(), Icon = "fas fa-tag" };
                    await _unitOfWork.GetRepository<StoreCategory>().AddAsync(newCat);
                    await _unitOfWork.SaveChangesAsync();
                    finalCategoryId = newCat.Id;
                }
            }

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
                Flavor = model.Flavor,
                StoreCategoryId = finalCategoryId,
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
            entity.Flavor = model.Flavor;

            // Handle dynamic category update
            if (!string.IsNullOrWhiteSpace(model.NewCategoryName))
            {
                var existingCat = (await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync(c => c.Name.ToLower() == model.NewCategoryName.Trim().ToLower())).FirstOrDefault();
                if (existingCat != null)
                {
                    entity.StoreCategoryId = existingCat.Id;
                }
                else
                {
                    var newCat = new StoreCategory { Name = model.NewCategoryName.Trim(), Icon = "fas fa-tag" };
                    await _unitOfWork.GetRepository<StoreCategory>().AddAsync(newCat);
                    await _unitOfWork.SaveChangesAsync();
                    entity.StoreCategoryId = newCat.Id;
                }
            }
            else
            {
                entity.StoreCategoryId = model.StoreCategoryId;
            }

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

                int availableStock = product.StockQuantity;
                decimal unitPrice = product.SalePrice ?? product.Price;

                if (item.StoreProductVariantId.HasValue)
                {
                    var variant = await _unitOfWork.GetRepository<StoreProductVariant>().GetByIdAsync(item.StoreProductVariantId.Value);
                    if (variant == null)
                        return $"Variant not found for product \"{product.Name}\".";
                    availableStock = variant.StockQuantity;
                    unitPrice += variant.PriceAdjustment;
                }

                if (item.Quantity > availableStock)
                    return $"Insufficient stock for \"{product.Name}\": requested {item.Quantity}, available {availableStock}.";

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

            var saleItemsData = new List<(StoreProduct Product, int Quantity, decimal UnitPrice, decimal LineTotal, decimal GrossProfit, StoreProductVariant? Variant)>();
            decimal totalBeforeDiscount = 0;

            foreach (var item in itemsToProcess)
            {
                var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(item.StoreProductId);
                if (product == null || !product.IsAvailable)
                    throw new InvalidOperationException($"Product ID {item.StoreProductId} is not available.");

                StoreProductVariant? variant = null;
                if (item.StoreProductVariantId.HasValue)
                {
                    variant = await _unitOfWork.GetRepository<StoreProductVariant>().GetByIdAsync(item.StoreProductVariantId.Value);
                }

                int requestedQty = item.Quantity;
                int availableQty = variant != null ? variant.StockQuantity : product.StockQuantity;
                if (availableQty < requestedQty)
                    throw new InvalidOperationException($"Insufficient stock for \"{product.Name}\": requested {requestedQty}, available {availableQty}.");

                var basePrice = product.SalePrice ?? product.Price;
                var unitPrice = basePrice + (variant?.PriceAdjustment ?? 0);
                var lineTotal = unitPrice * requestedQty;
                var grossProfit = (unitPrice - product.CostPrice) * requestedQty;
                totalBeforeDiscount += lineTotal;
                saleItemsData.Add((product, requestedQty, unitPrice, lineTotal, grossProfit, variant));
            }

            string? memberName = null;
            string? memberStatus = null;
            Member? memberEntity = null;
            if (model.MemberId.HasValue)
            {
                memberEntity = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.MemberId.Value);
                if (memberEntity != null)
                {
                    memberName = memberEntity.Name;
                    var lastSub = (await _unitOfWork.GetRepository<MemberShip>().GetAllAsync())
                        .Where(ms => ms.MemberId == model.MemberId.Value)
                        .OrderByDescending(ms => ms.EndDate).FirstOrDefault();
                    memberStatus = lastSub?.Status ?? "No Membership";
                }
            }

            decimal totalProfit = 0;
            decimal totalDiscount = model.DiscountAmount;
            decimal pointsRedeemedValue = 0;
            decimal pointsToDeduct = 0;
            Coupon? appliedCoupon = null;

            if (!string.IsNullOrWhiteSpace(model.CouponCode))
            {
                appliedCoupon = await ValidateCouponAsync(model.CouponCode, totalBeforeDiscount);
                if (appliedCoupon != null)
                {
                    decimal couponDiscount = appliedCoupon.DiscountType == DiscountType.Percentage 
                        ? totalBeforeDiscount * (appliedCoupon.Value / 100) : appliedCoupon.Value;
                    totalDiscount += couponDiscount;
                }
            }

            if (model.MemberId.HasValue && model.RedeemPoints && memberEntity != null && memberEntity.LoyaltyPoints > 0)
            {
                decimal maxDiscountFromPoints = memberEntity.LoyaltyPoints / 10;
                decimal remainingTotal = totalBeforeDiscount - totalDiscount;
                pointsRedeemedValue = Math.Min(maxDiscountFromPoints, remainingTotal);
                pointsToDeduct = pointsRedeemedValue * 10;
                totalDiscount += pointsRedeemedValue;
            }

            var saleItems = new List<SaleItem>();
            foreach (var data in saleItemsData)
            {
                decimal itemShareOfDiscount = totalBeforeDiscount > 0 ? (data.LineTotal / totalBeforeDiscount) * totalDiscount : 0;
                var itemNetProfit = data.GrossProfit - itemShareOfDiscount;
                totalProfit += itemNetProfit;

                if (data.Variant != null) { data.Variant.StockQuantity -= data.Quantity; _unitOfWork.GetRepository<StoreProductVariant>().Update(data.Variant); }
                else { data.Product.StockQuantity -= data.Quantity; _unitOfWork.GetRepository<StoreProduct>().Update(data.Product); }

                saleItems.Add(new SaleItem { StoreProductId = data.Product.Id, StoreProductVariantId = data.Variant?.Id, Quantity = data.Quantity, UnitPrice = data.UnitPrice, SnapshotCostPrice = data.Product.CostPrice, TotalPrice = data.LineTotal, ItemProfit = itemNetProfit });

                if (data.Product.StockQuantity <= data.Product.LowStockThreshold)
                {
                    try { await _notificationService.CreateNotificationAsync($"⚠️ Low Stock: {data.Product.Name}", $"Only {data.Product.StockQuantity} remaining.", "/Store/Index", null, "warning"); await _broadcastService.SendInventoryAlertAsync(data.Product.Id, data.Product.Name, data.Product.StockQuantity); } catch { }
                }
            }

            var invoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            var sale = new Sale 
            { 
                SaleDate = DateTime.Now, 
                TotalBeforeDiscount = totalBeforeDiscount, 
                TotalDiscount = totalDiscount, 
                NetTotal = totalBeforeDiscount - totalDiscount, 
                TotalProfit = totalProfit, 
                MemberId = model.MemberId, 
                MemberNameSnapshot = memberName, 
                MemberStatusSnapshot = memberStatus, 
                CustomerName = model.CustomerName, 
                PaymentMethod = model.PaymentMethod, 
                Notes = model.Notes, 
                Items = saleItems,
                InvoiceNumber = invoiceNumber,
                CashierName = model.CashierName,
                CashierEmail = model.CashierEmail
            };

            if (model.MemberId.HasValue)
            {
                var pointsEarned = await CalculateOrderPointsAsync(sale.NetTotal);
                sale.PointsEarned = pointsEarned;
                sale.PointsRedeemedValue = pointsRedeemedValue;
                sale.CouponCode = appliedCoupon?.Code;
                if (memberEntity != null) { memberEntity.LoyaltyPoints = (memberEntity.LoyaltyPoints - pointsToDeduct) + pointsEarned; _unitOfWork.GetRepository<Member>().Update(memberEntity); }
            }

            if (appliedCoupon != null) { appliedCoupon.UsedCount++; _unitOfWork.GetRepository<Coupon>().Update(appliedCoupon); }

            _unitOfWork.GetRepository<Sale>().Add(sale);
            await _unitOfWork.SaveChangesAsync();

            return sale.Id;
        }

        public async Task<bool> ProcessRefundAsync(int saleId, List<(int SaleItemId, int Quantity)> itemsToRefund)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var sale = await _unitOfWork.GetRepository<Sale>().GetByIdAsync(saleId);
                if (sale == null || sale.Status == SaleStatus.Voided) return false;

                // Fetch ALL items of this sale WITH tracking (by fetching by ID)
                var allItemsNoTracking = (await _unitOfWork.GetRepository<SaleItem>().GetAllAsync())
                    .Where(i => i.SaleId == saleId).ToList();
                
                var allItems = new List<SaleItem>();
                foreach (var itemNT in allItemsNoTracking)
                {
                    var trackedItem = await _unitOfWork.GetRepository<SaleItem>().GetByIdAsync(itemNT.Id);
                    if (trackedItem != null) allItems.Add(trackedItem);
                }

                foreach (var refundReq in itemsToRefund)
                {
                    var item = allItems.FirstOrDefault(i => i.Id == refundReq.SaleItemId);
                    if (item == null) continue;

                    int availableToRefund = item.Quantity - item.RefundedQuantity;
                    int refundQty = Math.Min(refundReq.Quantity, availableToRefund);
                    if (refundQty <= 0) continue;

                    // Update stock information
                    if (item.StoreProductVariantId.HasValue)
                    {
                        var variant = await _unitOfWork.GetRepository<StoreProductVariant>().GetByIdAsync(item.StoreProductVariantId.Value);
                        if (variant != null) 
                        { 
                            variant.StockQuantity += refundQty; 
                            _unitOfWork.GetRepository<StoreProductVariant>().Update(variant); 
                        }
                    }
                    else
                    {
                        var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(item.StoreProductId);
                        if (product != null) 
                        { 
                            product.StockQuantity += refundQty; 
                            _unitOfWork.GetRepository<StoreProduct>().Update(product); 
                        }
                    }

                    // Update item refund quantity
                    item.RefundedQuantity += refundQty;
                    _unitOfWork.GetRepository<SaleItem>().Update(item);
                }

                // Recalculate status based on the current state of tracked items
                bool allRefunded = allItems.All(i => i.RefundedQuantity >= i.Quantity);
                bool partiallyRefunded = allItems.Any(i => i.RefundedQuantity > 0);

                if (allRefunded)
                {
                    sale.Status = SaleStatus.Voided;
                    // Restore Loyalty Points if it was a member sale
                    if (sale.MemberId.HasValue)
                    {
                        var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(sale.MemberId.Value);
                        if (member != null)
                        {
                            // Logic: points given back = redeemed pts * 10, then remove earned pts
                            decimal pointsToReturn = (sale.PointsRedeemedValue * 10) - sale.PointsEarned;
                            member.LoyaltyPoints += pointsToReturn;
                            if (member.LoyaltyPoints < 0) member.LoyaltyPoints = 0;
                            
                            _unitOfWork.GetRepository<Member>().Update(member);
                        }
                    }
                }
                else if (partiallyRefunded)
                {
                    sale.Status = SaleStatus.PartiallyRefunded;
                }

                _unitOfWork.GetRepository<Sale>().Update(sale);
                return true;
            });
        }

        public async Task<PagedResult<SaleDetailsViewModel>> GetSalesHistoryAsync(int pageNumber = 1, int pageSize = 10, DateTime? from = null, DateTime? to = null, string? search = null)
        {
            Expression<Func<Sale, bool>>? filter = null;
            if (from.HasValue && to.HasValue)
            {
                var toDate = to.Value.AddDays(1);
                filter = s => s.SaleDate >= from.Value && s.SaleDate <= toDate;
            }
            else if (from.HasValue)
            {
                filter = s => s.SaleDate >= from.Value;
            }
            else if (to.HasValue)
            {
                var toDate = to.Value.AddDays(1);
                filter = s => s.SaleDate <= toDate;
            }

            if (!string.IsNullOrEmpty(search))
            {
                var originalFilter = filter;
                if (originalFilter != null)
                    filter = s => (s.CustomerName != null && s.CustomerName.Contains(search)) || (s.Notes != null && s.Notes.Contains(search)) || (s.InvoiceNumber != null && s.InvoiceNumber.Contains(search));
                else
                    filter = s => (s.CustomerName != null && s.CustomerName.Contains(search)) || (s.Notes != null && s.Notes.Contains(search)) || (s.InvoiceNumber != null && s.InvoiceNumber.Contains(search));
            }

            var pagedSales = await _unitOfWork.GetRepository<Sale>().GetPagedAsync(
                pageNumber, pageSize, filter,
                query => query.OrderByDescending(s => s.SaleDate));

            var saleIds = pagedSales.Items.Select(s => s.Id).ToList();
            var items = await _unitOfWork.GetRepository<SaleItem>().GetAllAsync(i => saleIds.Contains(i.SaleId));
            var saleItemsList = items.ToList();

            var prodIds = saleItemsList.Select(i => i.StoreProductId).Distinct().ToList();
            var variantIds = saleItemsList.Where(v => v.StoreProductVariantId.HasValue).Select(v => v.StoreProductVariantId!.Value).Distinct().ToList();
            var memberIds = pagedSales.Items.Where(s => s.MemberId.HasValue).Select(s => s.MemberId!.Value).Distinct().ToList();

            var prodDict = (await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync(p => prodIds.Contains(p.Id))).ToDictionary(p => p.Id);
            var variantDict = (await _unitOfWork.GetRepository<StoreProductVariant>().GetAllAsync(v => variantIds.Contains(v.Id))).ToDictionary(v => v.Id);
            var memberDict = (await _unitOfWork.GetRepository<Member>().GetAllAsync(m => memberIds.Contains(m.Id))).ToDictionary(m => m.Id);
            
            var itemsBySale = saleItemsList.GroupBy(i => i.SaleId).ToDictionary(g => g.Key, g => g.ToList());

            return new PagedResult<SaleDetailsViewModel>
            {
                PageNumber = pagedSales.PageNumber,
                PageSize = pagedSales.PageSize,
                TotalCount = pagedSales.TotalCount,
                Items = pagedSales.Items.Select(s =>
                {
                    var saleItems = itemsBySale.TryGetValue(s.Id, out var si) ? si : new List<SaleItem>();
                    string? mName = null;
                    if (s.MemberId.HasValue && memberDict.TryGetValue(s.MemberId.Value, out var mem)) mName = mem.Name;
                    
                    return new SaleDetailsViewModel
                    {
                        Id = s.Id,
                        SaleDate = s.SaleDate,
                        TotalBeforeDiscount = s.TotalBeforeDiscount,
                        TotalDiscount = s.TotalDiscount,
                        NetTotal = s.NetTotal,
                        TotalProfit = s.TotalProfit,
                        CustomerName = s.CustomerName,
                        MemberName = mName ?? s.MemberNameSnapshot,
                        PaymentMethod = s.PaymentMethod,
                        Notes = s.Notes,
                        Items = saleItems.Select(i => new SaleItemDetailViewModel
                        {
                            Id = i.Id,
                            ProductName = prodDict.TryGetValue(i.StoreProductId, out var pr) ? pr.Name : "?",
                            VariantName = i.StoreProductVariantId.HasValue && variantDict.TryGetValue(i.StoreProductVariantId.Value, out var vr)
                                ? vr.VariantName : null,
                            ProductImageUrl = pr?.ImageUrl,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            SnapshotCostPrice = i.SnapshotCostPrice,
                            TotalPrice = i.TotalPrice,
                            ItemProfit = i.ItemProfit,
                            RefundedQuantity = i.RefundedQuantity
                        }).ToList(),
                        InvoiceNumber = s.InvoiceNumber,
                        CashierName = s.CashierName,
                        CashierEmail = s.CashierEmail,
                        Status = s.Status
                    };
                }).ToList()
            };
        }

        public async Task<SaleDetailsViewModel?> GetSaleDetailsAsync(int id)
        {
            var sale = await _unitOfWork.GetRepository<Sale>().GetByIdAsync(id);
            if (sale == null) return null;

            var items = (await _unitOfWork.GetRepository<SaleItem>().GetAllAsync(i => i.SaleId == id)).ToList();
            
            var prodIds = items.Select(i => i.StoreProductId).Distinct().ToList();
            var variantIds = items.Where(v => v.StoreProductVariantId.HasValue).Select(v => v.StoreProductVariantId!.Value).Distinct().ToList();
            
            var products = (await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync(p => prodIds.Contains(p.Id))).ToDictionary(p => p.Id);
            var variants = (await _unitOfWork.GetRepository<StoreProductVariant>().GetAllAsync(v => variantIds.Contains(v.Id))).ToDictionary(v => v.Id);

            string? memberName = null;
            if (sale.MemberId.HasValue)
            {
                var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(sale.MemberId.Value);
                memberName = member?.Name;
            }

            return new SaleDetailsViewModel
            {
                Id = sale.Id,
                SaleDate = sale.SaleDate,
                TotalBeforeDiscount = sale.TotalBeforeDiscount,
                TotalDiscount = sale.TotalDiscount,
                NetTotal = sale.NetTotal,
                TotalProfit = sale.TotalProfit,
                CustomerName = sale.CustomerName,
                MemberName = memberName ?? sale.MemberNameSnapshot,
                PaymentMethod = sale.PaymentMethod,
                Notes = sale.Notes,
                PointsEarned = sale.PointsEarned,
                PointsRedeemedValue = sale.PointsRedeemedValue,
                CouponCode = sale.CouponCode,
                InvoiceNumber = sale.InvoiceNumber,
                CashierName = sale.CashierName,
                CashierEmail = sale.CashierEmail,
                Status = sale.Status,
                Items = items.Select(i => new SaleItemDetailViewModel
                {
                    Id = i.Id,
                    ProductName = products.TryGetValue(i.StoreProductId, out var pr) ? pr.Name : "?",
                    VariantName = i.StoreProductVariantId.HasValue && variants.TryGetValue(i.StoreProductVariantId.Value, out var vr) ? vr.VariantName : null,
                    ProductImageUrl = products.TryGetValue(i.StoreProductId, out var p) ? p.ImageUrl : null,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SnapshotCostPrice = i.SnapshotCostPrice,
                    TotalPrice = i.TotalPrice,
                    ItemProfit = i.ItemProfit,
                    RefundedQuantity = i.RefundedQuantity
                }).ToList()
            };
        }

        // ── Dashboard & Reporting ─────────────────────────────────────────────
        public async Task<StoreDashboardStatsViewModel> GetDashboardStatsAsync()
        {
            var sales = await _unitOfWork.GetRepository<Sale>().GetAllAsync();
            var prods = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var items = await _unitOfWork.GetRepository<SaleItem>().GetAllAsync();
            var cats = await _unitOfWork.GetRepository<StoreCategory>().GetAllAsync();

            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var thirtyDaysAgo = today.AddDays(-29);
            var expiryThreshold = today.AddDays(30);

            var catDict = cats.ToDictionary(c => c.Id, c => c.Name);
            var prodToCat = prods.ToDictionary(p => p.Id, p => p.StoreCategoryId);

            // Basic Stats
            var stats = new StoreDashboardStatsViewModel
            {
                TodayRevenue = sales.Where(s => s.SaleDate.Date == today && s.Status != SaleStatus.Voided).Sum(s => s.NetTotal),
                MonthRevenue = sales.Where(s => s.SaleDate >= monthStart && s.Status != SaleStatus.Voided).Sum(s => s.NetTotal),
                MonthProfit = sales.Where(s => s.SaleDate >= monthStart && s.Status != SaleStatus.Voided).Sum(s => s.TotalProfit),
                TotalProducts = prods.Count(),
                LowStockCount = prods.Count(p => p.StockQuantity <= p.LowStockThreshold),
                TotalSalesToday = sales.Count(s => s.SaleDate.Date == today && s.Status != SaleStatus.Voided),
                ExpiringCount = prods.Count(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value <= expiryThreshold && p.ExpiryDate.Value >= today)
            };

            // 1. Daily Revenue (Last 30 Days)
            var dailySales = sales.Where(s => s.SaleDate >= thirtyDaysAgo && s.Status != SaleStatus.Voided)
                .GroupBy(s => s.SaleDate.Date)
                .ToDictionary(g => g.Key.ToString("MM-dd"), g => g.Sum(s => s.NetTotal));

            for (int i = 0; i < 30; i++)
            {
                var dateStr = thirtyDaysAgo.AddDays(i).ToString("MM-dd");
                stats.DailyRevenueLast30Days[dateStr] = dailySales.TryGetValue(dateStr, out var val) ? val : 0;
            }

            // 2. Category Distribution
            stats.CategorySalesDistribution = items
                .Where(i => prodToCat.ContainsKey(i.StoreProductId))
                .GroupBy(i => catDict.TryGetValue(prodToCat[i.StoreProductId], out var n) ? n : "Misc")
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

            // 3. Top Products by Profit
            stats.TopProductsByProfit = items
                .GroupBy(i => i.StoreProductId)
                .Select(g => new BestSellerViewModel
                {
                    ProductName = prods.FirstOrDefault(p => p.Id == g.Key)?.Name ?? "Unknown",
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice),
                    TotalProfit = g.Sum(x => x.ItemProfit)
                })
                .OrderByDescending(x => x.TotalProfit)
                .Take(5)
                .ToList();

            return stats;
        }

        public async Task<StoreReportViewModel> GetStoreReportAsync(DateTime from, DateTime to)
        {
            var pagedSales = await GetSalesHistoryAsync(1, 10000, from, to);
            var salesList = pagedSales.Items.ToList();
            
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

            // Ensure correct sign based on type
            if (model.AdjustmentType == StockAdjustmentType.Damage || model.AdjustmentType == StockAdjustmentType.Loss)
            {
                model.Quantity = -Math.Abs(model.Quantity);
            }
            else if (model.AdjustmentType == StockAdjustmentType.Addition || model.AdjustmentType == StockAdjustmentType.Return)
            {
                model.Quantity = Math.Abs(model.Quantity);
            }
            // Correction (4) can be either positive or negative as sent by user

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

        public async Task QuickStockAddAsync(int productId, int amount)
        {
            await CreateStockAdjustmentAsync(new StockAdjustmentViewModel
            {
                ProductId = productId,
                Quantity = amount,
                AdjustmentType = StockAdjustmentType.Addition,
                Reason = "Quick Add from Low Stock Panel"
            });
        }

        // ── Expiry ─────────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreProductViewModel>> GetExpiringProductsAsync(int withinDays = 30)
        {
            var paged = await GetAllProductsAsync(1, 1000); // Fetch a large batch for alerts
            var products = paged.Items;
            var threshold = DateTime.Today.AddDays(withinDays);
            return products.Where(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value <= threshold && p.ExpiryDate.Value >= DateTime.Today)
                           .OrderBy(p => p.ExpiryDate);
        }

        public async Task<IEnumerable<StoreProductViewModel>> GetLowStockProductsAsync()
        {
            var paged = await GetAllProductsAsync(1, 1000);
            var products = paged.Items;
            return products.Where(p => p.IsLowStock)
                           .OrderBy(p => p.StockQuantity);
        }

        // ── Variants ───────────────────────────────────────────────────────────
        public async Task<IEnumerable<StoreProductVariantViewModel>> GetVariantsByProductIdAsync(int productId)
        {
            var variants = await _unitOfWork.GetRepository<StoreProductVariant>().GetAllAsync();
            return variants.Where(v => v.StoreProductId == productId)
                .Select(v => new StoreProductVariantViewModel
                {
                    Id = v.Id,
                    VariantName = v.VariantName,
                    SKU = v.SKU,
                    PriceAdjustment = v.PriceAdjustment,
                    StockQuantity = v.StockQuantity
                }).ToList();
        }

        public async Task CreateVariantAsync(StoreProductVariantViewModel model, int productId)
        {
            var variant = new StoreProductVariant
            {
                StoreProductId = productId,
                VariantName = model.VariantName,
                SKU = model.SKU,
                PriceAdjustment = model.PriceAdjustment,
                StockQuantity = model.StockQuantity
            };
            await _unitOfWork.GetRepository<StoreProductVariant>().AddAsync(variant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateVariantAsync(StoreProductVariantViewModel model)
        {
            var variant = await _unitOfWork.GetRepository<StoreProductVariant>().GetByIdAsync(model.Id);
            if (variant == null) return;

            variant.VariantName = model.VariantName;
            variant.SKU = model.SKU;
            variant.PriceAdjustment = model.PriceAdjustment;
            variant.StockQuantity = model.StockQuantity;

            _unitOfWork.GetRepository<StoreProductVariant>().Update(variant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteVariantAsync(int id)
        {
            var variant = await _unitOfWork.GetRepository<StoreProductVariant>().GetByIdAsync(id);
            if (variant == null) return;

            _unitOfWork.GetRepository<StoreProductVariant>().Delete(variant);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<BulkImportResultViewModel> BulkImportProductsAsync(BulkImportRequestViewModel model)
        {
            var result = new BulkImportResultViewModel { IsDryRun = model.IsDryRun };

            if (model.IsDryRun)
            {
                await PerformBulkImportLogic(model, result);
            }
            else
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await PerformBulkImportLogic(model, result);
                    return true;
                });
            }

            return result;
        }

        private async Task PerformBulkImportLogic(BulkImportRequestViewModel model, BulkImportResultViewModel result)
        {
            using var stream = model.File.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); 

            var categories = await GetAllCategoriesAsync();
            var products = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var existingSkus = products.Where(p => !string.IsNullOrEmpty(p.SKU)).Select(p => p.SKU!).ToHashSet();

            int rowIndex = 2; 
            foreach (var row in rows)
            {
                result.TotalRows++;
                var name = row.Cell(1).GetValue<string>();
                var sku = row.Cell(2).GetValue<string>();
                var priceStr = row.Cell(3).GetValue<string>();
                var costPriceStr = row.Cell(4).GetValue<string>();
                var stockStr = row.Cell(5).GetValue<string>();
                var categoryName = row.Cell(6).GetValue<string>();

                var rowErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(name)) rowErrors.Add("Product Name is required.");
                if (!decimal.TryParse(priceStr, out decimal price) || price < 0) rowErrors.Add("Invalid Price.");
                if (!decimal.TryParse(costPriceStr, out decimal costPrice) || costPrice < 0) rowErrors.Add("Invalid Cost Price.");
                if (!int.TryParse(stockStr, out int stock) || stock < 0) rowErrors.Add("Invalid Stock Quantity.");

                var category = categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                if (category == null) rowErrors.Add($"Category '{categoryName}' not found.");

                if (!string.IsNullOrEmpty(sku) && existingSkus.Contains(sku))
                    rowErrors.Add($"SKU '{sku}' already exists.");

                if (rowErrors.Any())
                {
                    result.FailureCount++;
                    result.Errors.Add(new ImportRowError { RowIndex = rowIndex, ProductName = name ?? "Unknown", Messages = rowErrors });
                }
                else
                {
                    if (!model.IsDryRun)
                    {
                        var newProduct = new StoreProduct
                        {
                            Name = name,
                            SKU = sku,
                            Price = price,
                            CostPrice = costPrice,
                            StockQuantity = stock,
                            StoreCategoryId = category!.Id,
                            IsAvailable = true
                        };
                        _unitOfWork.GetRepository<StoreProduct>().Add(newProduct);
                        if (!string.IsNullOrEmpty(sku)) existingSkus.Add(sku);
                    }
                    result.SuccessCount++;
                }
                rowIndex++;
            }
            if (!model.IsDryRun) await _unitOfWork.SaveChangesAsync();
        }

        // ── Marketing & Loyalty ────────────────────────────────────────────────
        public async Task<decimal> GetMemberPointsAsync(int memberId)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId);
            return member?.LoyaltyPoints ?? 0;
        }

        public async Task<Coupon?> ValidateCouponAsync(string code, decimal currentTotal)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var lowerCode = code.ToLower();
            var coupon = (await _unitOfWork.GetRepository<Coupon>().GetAllAsync(c => c.Code.ToLower() == lowerCode && c.IsActive))
                .FirstOrDefault();

            if (coupon == null) return null;

            // Check expiry
            if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.Now)
                return null;

            // Check usage limit
            if (coupon.UsedCount >= coupon.UsageLimit)
                return null;

            // Check minimum amount
            if (coupon.MinimumAmount.HasValue && currentTotal < coupon.MinimumAmount.Value)
                return null;

            return coupon;
        }

        public async Task<decimal> CalculateOrderPointsAsync(decimal netTotal)
        {
            // Simple rule: 1 EGP = 1 Point
            return Math.Floor(netTotal);
        }

        // ── Purchases ─────────────────────────────────────────────────────────
        public async Task<IEnumerable<StorePurchaseHistoryViewModel>> GetPurchaseHistoryAsync(int? productId = null, int? supplierId = null, DateTime? from = null, DateTime? to = null)
        {
            var purchases = await _unitOfWork.GetRepository<StorePurchase>().GetAllAsync();
            var products = await _unitOfWork.GetRepository<StoreProduct>().GetAllAsync();
            var suppliers = await _unitOfWork.GetRepository<Supplier>().GetAllAsync();

            var prodDict = products.ToDictionary(p => p.Id, p => p.Name);
            var supplDict = suppliers.ToDictionary(s => s.Id, s => s.Name);

            var query = purchases.AsEnumerable();
            if (productId.HasValue) query = query.Where(p => p.ProductId == productId.Value);
            if (supplierId.HasValue) query = query.Where(p => p.SupplierId == supplierId.Value);
            if (from.HasValue) query = query.Where(p => p.PurchaseDate >= from.Value);
            if (to.HasValue) query = query.Where(p => p.PurchaseDate <= to.Value.AddDays(1));

            return query.OrderByDescending(p => p.PurchaseDate)
                .Select(p => new StorePurchaseHistoryViewModel
                {
                    Id = p.Id,
                    ProductName = prodDict.TryGetValue(p.ProductId, out var pn) ? pn : "?",
                    SupplierName = p.SupplierId.HasValue && supplDict.TryGetValue(p.SupplierId.Value, out var sn) ? sn : "General",
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    TotalPrice = p.TotalPrice,
                    PurchaseDate = p.PurchaseDate,
                    Notes = p.Notes,
                    ExpenseId = p.ExpenseId
                }).ToList();
        }

        public async Task CreatePurchaseAsync(StorePurchaseViewModel model)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var product = await _unitOfWork.GetRepository<StoreProduct>().GetByIdAsync(model.ProductId);
                if (product == null) return false;

                // 1. Update stock and cost price
                product.StockQuantity += model.Quantity;
                product.CostPrice = model.UnitPrice; // Update cost price to latest purchase price
                _unitOfWork.GetRepository<StoreProduct>().Update(product);

                // 2. Create Expense if requested
                int? expenseId = null;
                if (model.CreateExpenseRecord)
                {
                    var isAr = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar";
                    var expense = new Expense
                    {
                        Title = isAr 
                            ? $"شراء مخزون: {product.NameAr ?? product.Name} (كمية: {model.Quantity})" 
                            : $"Stock Purchase: {product.Name} (Qty: {model.Quantity})",
                        Amount = model.TotalPrice,
                        Date = model.PurchaseDate,
                        Category = "StorePurchase",
                        Notes = isAr 
                            ? $"شراء من المورد رقم: {model.SupplierId}. {model.Notes}"
                            : $"Purchased from supplier ID: {model.SupplierId}. {model.Notes}"
                    };
                    await _unitOfWork.GetRepository<Expense>().AddAsync(expense);
                    await _unitOfWork.SaveChangesAsync(); // Need ID for FK
                    expenseId = expense.Id;
                }

                // 3. Create StorePurchase record
                var purchase = new StorePurchase
                {
                    ProductId = model.ProductId,
                    SupplierId = model.SupplierId,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice,
                    TotalPrice = model.TotalPrice,
                    PurchaseDate = model.PurchaseDate,
                    Notes = model.Notes,
                    ExpenseId = expenseId
                };
                await _unitOfWork.GetRepository<StorePurchase>().AddAsync(purchase);
                
                // 4. Also create a StockAdjustment record for visibility in history
                var adjustment = new StockAdjustment
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    AdjustmentType = StockAdjustmentType.Addition,
                    Reason = $"Purchase from Supplier. Store Purchase Record linked.",
                    AdjustmentDate = model.PurchaseDate
                };
                await _unitOfWork.GetRepository<StockAdjustment>().AddAsync(adjustment);

                await _unitOfWork.SaveChangesAsync();
                return true;
            });
        }
    }
}
