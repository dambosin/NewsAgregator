using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewsAgregator.Abstractions;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Parsers;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Data;
using NewsAgregator.Data.Entities;
using NewsAgregator.Repository;
using NewsAgregator.Repository.Implemintation;
using Serilog;

namespace NewsAgregator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .Build();
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<NewsAgregatorContext>(
                opt =>
                {
                    var connString = builder.Configuration
                        .GetConnectionString("DefaultConnection");
                    opt.UseSqlServer(connString);
                });
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

            builder.Services.AddScoped<ISiteParser, OnlinerParser>();
            builder.Services.AddScoped<ISiteParser, TutbyParser>();
            builder.Services.AddScoped<ISiteParser, DtfParseer>();
            builder.Services.AddTransient<ISiteParserFactory, SiteParserFactory>();


            builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
            builder.Services.AddScoped<IRepository<Like>, Repository<Like>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<UserRole>, Repository<UserRole>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            builder.Services.AddTransient<IArticleService, ArticleService>();
            builder.Services.AddTransient<ISourceService, SourceService>();
            builder.Services.AddTransient<ICommentService, CommentService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddTransient<IRateService, RateService>();

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddHangfire(config => config
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();

            builder.Services.AddControllersWithViews();

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Account/AccessDenied");
                });

            builder.Services.AddRazorPages();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHangfireDashboard();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHangfireDashboard("/hangfire");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}