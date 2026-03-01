using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading;

namespace GymManagmentDAL.Data.Context
{
    public class GymDBContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GymDBContext(DbContextOptions<GymDBContext> option, IHttpContextAccessor httpContextAccessor = null!) : base(option)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            modelBuilder.Entity<ApplicationUser>(E =>
            {
                E.Property(x => x.FirstName).HasColumnType("nvarchar").HasMaxLength(50);
                E.Property(x => x.LastName).HasColumnType("nvarchar").HasMaxLength(50);
            });

            modelBuilder.Entity<HealthRecord>(e =>
            {
                e.Property(h => h.Weight).HasColumnType("decimal(18,2)");
                e.Property(h => h.Height).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<HealthProgress>(e =>
            {
                e.Property(h => h.Weight).HasColumnType("decimal(18,2)");
                e.Property(h => h.Height).HasColumnType("decimal(18,2)");
                e.Property(h => h.BMI).HasColumnType("decimal(18,2)");
                e.Property(h => h.BodyFatPercentage).HasColumnType("decimal(18,2)");
                e.Property(h => h.MuscleMass).HasColumnType("decimal(18,2)");
            });

            // Global Query Filter for Soft Delete
            var entityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType));

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }

            modelBuilder.Entity<StoreProduct>(e =>
            {
                e.HasIndex(p => p.SKU).IsUnique().HasFilter("[IsDeleted] = 0");
                e.HasIndex(p => p.Barcode);
            });

            modelBuilder.Entity<StoreProductVariant>(e =>
            {
                e.HasIndex(v => v.SKU).IsUnique().HasFilter("[IsDeleted] = 0");
            });

            modelBuilder.Entity<Sale>(e =>
            {
                e.HasIndex(s => s.SaleDate);
                e.HasIndex(s => s.MemberId);
                e.HasIndex(s => s.CouponCode);
            });

            modelBuilder.Entity<Coupon>(e =>
            {
                e.HasIndex(c => c.Code).IsUnique().HasFilter("[IsDeleted] = 0");
            });
        }

        private static dynamic ConvertFilterExpression(Type type)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var constant = System.Linq.Expressions.Expression.Constant(false);
            var body = System.Linq.Expressions.Expression.Equal(property, constant);
            return System.Linq.Expressions.Expression.Lambda(body, parameter);
        }

        private void HandleGlobalProperties()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            var now = DateTime.Now;
            var userId = _httpContextAccessor?.HttpContext?.User?.Identity?.Name;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = now;
                    entity.UpdatedAt = now;
                    entity.CreatedByUserId = userId;
                    entity.UpdatedByUserId = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = now;
                    entity.UpdatedByUserId = userId;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = now;
                    entity.DeletedByUserId = userId;
                    entity.UpdatedAt = now;
                    entity.UpdatedByUserId = userId;
                }
            }
        }

        private async Task HandleAuditing()
        {
            var auditEntries = new List<AuditEntry>();
            var userId = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            var entries = ChangeTracker.Entries()
                .Where(e => !(e.Entity is AuditLog) && e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            foreach (var entry in entries)
            {
                var auditEntry = new AuditEntry(entry)
                {
                    UserName = userId,
                    EntityName = entry.Entity.GetType().Name
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.EntityId = property.CurrentValue?.ToString() ?? "";
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.Action = "Create";
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            break;

                        case EntityState.Deleted:
                            auditEntry.Action = "Delete";
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.Action = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue!;
                                auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            }
                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAuditLog());
            }
        }

        public class AuditEntry
        {
            public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
            {
                Entry = entry;
            }
            public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
            public string UserName { get; set; } = null!;
            public string EntityName { get; set; } = null!;
            public string EntityId { get; set; } = null!;
            public string Action { get; set; } = null!;
            public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
            public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

            public AuditLog ToAuditLog()
            {
                var audit = new AuditLog();
                audit.UserName = UserName;
                audit.Action = Action;
                audit.EntityName = EntityName;
                audit.EntityId = EntityId;
                audit.Timestamp = DateTime.Now;
                audit.OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues);
                audit.NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues);
                return audit;
            }
        }

        public override int SaveChanges()
        {
            HandleGlobalProperties();
            HandleAuditing().Wait();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleGlobalProperties();
            await HandleAuditing();
            return await base.SaveChangesAsync(cancellationToken);
        }

        #region DBSet
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerSpecialty> TrainerSpecialties { get; set; }
        public DbSet<TrainerAttendance> TrainerAttendances { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<MemberShip> MemberShips { get; set; }
        public DbSet<MemberSession> MemberSessions { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<GymSettings> GymSettings { get; set; }
        public DbSet<HealthProgress> HealthProgresses { get; set; }
        public DbSet<WalkInBooking> WalkInBookings { get; set; }
        public DbSet<WalkInSession> WalkInSessions { get; set; }

        // Store Module
        public DbSet<StoreCategory> StoreCategories { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }
        public DbSet<StoreProductVariant> StoreProductVariants { get; set; }
        public DbSet<StoreProductImage> StoreProductImages { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<StorePurchase> StorePurchases { get; set; }
        #endregion
    }
}
