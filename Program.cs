using BankingSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using BankingSystem.Models;

namespace BankingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<BankingDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

                        builder.Services.AddDefaultIdentity<ApplicationUser>()
                .AddEntityFrameworkStores<BankingDbContext>().AddDefaultUI().AddDefaultTokenProviders();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();;

            app.UseAuthorization();
            app.MapRazorPages();

            app.MapControllerRoute(
            name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            


            app.Run();
        }
    }
}