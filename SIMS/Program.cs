using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SIMS.Interfaces;
using SIMS.Repositories;
using SIMS.Services;
using SIMS.SimsDbContext;

namespace SIMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure to connect datatabase
            builder.Services.AddDbContext<SimDbContext>(options => 
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"), sqlOptions =>
                {
                    // Disable OUTPUT clause for tables with triggers
                    sqlOptions.EnableRetryOnFailure();
                });
            });

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICourseRepository, CourseRepository>();
            builder.Services.AddScoped<IStudentCourseRepository, StudentCourseRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

            // Services
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ICourseService, CourseService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<IActivityLogService, ActivityLogService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Configure the Identity Authentication
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Login/Index";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
                options.LogoutPath = "/Login/Logout";
            });
            // Configure the Roles for user
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Student", policy => policy.RequireRole("Student"));
                options.AddPolicy("FalcultyOnly", policy => policy.RequireRole("Falculty"));
            });

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
