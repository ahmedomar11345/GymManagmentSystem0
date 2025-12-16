using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Reposotories.Interfaces;
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
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(IGenericRepository<>));
            builder.Services.AddScoped<IplaneRepository, IplaneRepository>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
