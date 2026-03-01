using GymManagmentBLL;
using GymManagmentBLL.Service.Classes;
using GymManagmentBLL.Service.Classes.AttachmentService;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Interfaces.AttachmentService;
using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Data.DataSeeding;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Classes;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using Microsoft.Extensions.Localization;
using GymManagmentPL.Hubs;
using GymManagmentPL.Services;

namespace GymManagmentPL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/gym-management-.log", 
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting Gym Management System...");
                
                var builder = WebApplication.CreateBuilder(args);
                
                // Add Serilog
                builder.Host.UseSerilog();

                // Add Localization
                builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

                builder.Services.AddHttpContextAccessor();

                // Add services to the container with global authorization
                builder.Services.AddControllersWithViews(options =>
                {
                    // Require authentication by default for all controllers
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                    
                    // Add global anti-forgery validation for non-API controllers
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(SharedResource));
                });

                // Configure ModelBinding localized error messages
                builder.Services.AddOptions<MvcOptions>()
                    .Configure<IStringLocalizer<SharedResource>>((options, localizer) =>
                    {
                        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => localizer["InvalidInput"]);
                        options.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor((x) => localizer["InvalidInput"]);
                        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor((x) => localizer["InvalidInput"]);
                        options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor((x) => localizer["Required"]);
                        options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor((x) => localizer["Required"]);
                        options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor((x) => localizer["InvalidInput"]);
                        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => localizer["Required"]);
                    });

                // Database Context
                builder.Services.AddDbContext<GymDBContext>(option =>
                {
                    option.UseSqlServer(
                        builder.Configuration.GetConnectionString("DefaultConnection"),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                errorNumbersToAdd: null);
                        });
                    
                    // Only enable sensitive logging in development
                    if (builder.Environment.IsDevelopment())
                    {
                        option.EnableSensitiveDataLogging();
                        option.EnableDetailedErrors();
                    }
                });

                // Repository & Unit of Work
                builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
                builder.Services.AddScoped<ISessionRepository, SessionRepository>();
                builder.Services.AddScoped<IMemberShipRepository, MemberShipRepository>();
                builder.Services.AddScoped<IMemberSessionRepository, MemberSessionRepository>();
                builder.Services.AddScoped<IMemberRepository, MemberRepository>();
                builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
                builder.Services.AddScoped<ITrainerSpecialtyRepository, TrainerSpecialtyRepository>();

                // Business Services
                builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
                builder.Services.AddScoped<IPlanService, PlanService>();
                builder.Services.AddScoped<ITrainerService, TrainerService>();
                builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<ITrainerSpecialtyService, TrainerSpecialtyService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddSingleton<IQRCodeService, QRCodeService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
                builder.Services.AddScoped<IMemberShipService, MemberShipService>();
                builder.Services.AddScoped<IMemberSessionService, MemberSessionService>();
                builder.Services.AddScoped<IAttachmentService, AttachmentService>();
                builder.Services.AddScoped<IAccountService, AccountService>();
                builder.Services.AddScoped<IEmailService, GymManagmentBLL.Service.Implementations.EmailService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IScheduledTaskService, ScheduledTaskService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IGymSettingsService, GymSettingsService>();
builder.Services.AddScoped<IWalkInService, WalkInService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IBroadcastService, BroadcastService>();

// Hosted Services
builder.Services.AddHostedService<GymManagmentPL.Services.MembershipRenewalWorker>();
builder.Services.AddHostedService<GymManagmentPL.Services.SessionCleanupWorker>();

// Anti-forgery for AJAX
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
});

builder.Services.AddSignalR();

                // Identity Configuration
                builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    // Password settings
                    config.Password.RequireDigit = true;
                    config.Password.RequireLowercase = true;
                    config.Password.RequireUppercase = true;
                    config.Password.RequireNonAlphanumeric = true;
                    config.Password.RequiredLength = 8;
                    
                    // Lockout settings
                    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    config.Lockout.MaxFailedAccessAttempts = 5;
                    config.Lockout.AllowedForNewUsers = true;
                    
                    // User settings
                    config.User.RequireUniqueEmail = true;
                    config.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    
                    // Sign-in settings
                    config.SignIn.RequireConfirmedAccount = false;
                    
                }).AddEntityFrameworkStores<GymDBContext>()
                  .AddDefaultTokenProviders();

                builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, GymManagmentPL.Services.ApplicationUserClaimsPrincipalFactory>();

                // Cookie configuration
                builder.Services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.LogoutPath = "/Account/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                });

                // Authorization Policies
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("SuperAdminOnly", policy => 
                        policy.RequireRole("SuperAdmin"));
                    
                    options.AddPolicy("AdminOrAbove", policy => 
                        policy.RequireRole("SuperAdmin", "Admin"));
                    
                    options.AddPolicy("CanManageMembers", policy => 
                        policy.RequireRole("SuperAdmin", "Admin"));
                    
                    options.AddPolicy("CanManageTrainers", policy => 
                        policy.RequireRole("SuperAdmin"));
                    
                    options.AddPolicy("CanManagePlans", policy => 
                        policy.RequireRole("SuperAdmin"));
                });

                // AutoMapper
                builder.Services.AddAutoMapper(config => config.AddProfile(new MappingProfile()));

                // Add Response Caching
                builder.Services.AddResponseCaching();
                
                // Add Memory Cache
                builder.Services.AddMemoryCache();

                // Health Checks
                builder.Services.AddHealthChecks()
                    .AddDbContextCheck<GymDBContext>("database");

                var app = builder.Build();

                #region DataSeeding
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<GymDBContext>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                    
                    var pendingMigration = dbContext.Database.GetPendingMigrations();
                    if (pendingMigration?.Any() ?? false)
                    {
                        Log.Information("Applying {Count} pending migrations...", pendingMigration.Count());
                        dbContext.Database.Migrate();
                    }
                    
                    GymDbContextSeeding.SeedData(dbContext, app.Environment.ContentRootPath);
                    await IdentityDbContextSeeding.SeedData(roleManager, userManager, configuration);
                }
                #endregion

                // Configure the HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
                    app.UseHsts();
                }

                // Security Headers Middleware
                app.Use(async (context, next) =>
                {
                    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                    context.Response.Headers.Append("X-Frame-Options", "DENY");
                    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                    context.Response.Headers.Append("Content-Security-Policy", 
                        "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com; " +
                        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
                        "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                        "img-src 'self' data: https:; media-src 'self' https://assets.mixkit.co;");
                    await next();
                });

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                
                // Localization middleware
                var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };
                app.UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("ar"),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });

                app.UseResponseCaching();
                app.UseAuthentication();
                app.UseAuthorization();

                // Health check endpoint
                app.MapHealthChecks("/health");

                app.MapHub<InventoryHub>("/inventoryHub");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");

                Log.Information("Gym Management System started successfully");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
