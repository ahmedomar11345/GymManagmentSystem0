using GymManagmentBLL;
using GymManagmentBLL.Service.Classes;
using GymManagmentBLL.Service.Classes.AttachmentService;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Interfaces.AttachmentService;
using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Data.DataSeeding;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Classes;
using GymManagmentDAL.Reposotories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagmentPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<GymDBContext>(option =>
            {
                //option.UseSqlServer(builder.Configuration.GetSection("GetConnectionString")["DefaultConnection"]);
                //option.UseSqlServer(builder.Configuration["ConnectionString:DefaultConnection"]);
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            }
            );
            //builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(IGenericRepository<>));
            //builder.Services.AddScoped<IplaneRepository, IplaneRepository>();
            builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository , SessionRepository>();
            builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IMemberShipService, MemberShipService>();
            builder.Services.AddScoped<IMemberSessionService, MemberSessionService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                
                config.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<GymDBContext>();
            builder.Services.ConfigureApplicationCookie(option =>
            {
                option.LoginPath = "/Account/Login";
                option.AccessDeniedPath = "/Account/AccessDenied";

            });
            builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));




            var app = builder.Build();

            #region DataSeeding
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GymDBContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(); 
            var pendingMigration = dbContext.Database.GetPendingMigrations();
            if (pendingMigration?.Any() ?? false)
            {
                dbContext.Database.Migrate();
            }
            GymDbContextSeeding.SeedData(dbContext, app.Environment.ContentRootPath);
            IdentityDbContextSeeding.SeedData( roleManager, userManager);
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); 

            app.UseAuthorization();

            app.MapStaticAssets();
                

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id:?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
