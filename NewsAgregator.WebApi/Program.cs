using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewsAgregator.Abstractions;
using NewsAgregator.Abstractions.Repository;
using NewsAgregator.Abstractions.Services;
using NewsAgregator.Buisness.Parsers;
using NewsAgregator.Buisness.Services;
using NewsAgregator.Data;
using NewsAgregator.Data.Cqs.QueriesHandlers.Article;
using NewsAgregator.Data.Entities;
using NewsAgregator.Repository;
using NewsAgregator.Repository.Implemintation;
using Serilog;
using System.Text;

namespace NewsAgregator.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
               .Build();
            builder.Services.AddDbContext<NewsAgregatorContext>(
                opt =>
                {
                    var connString = configuration
                        .GetConnectionString("DefaultConnection");
                    opt.UseSqlServer(connString);
                });
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            builder.Host.UseSerilog();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:SecurityKey"]!))
                    };
                });

            builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetArticlesByPageQueryHandler>());

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

            builder.Services.AddTransient<IArticleService, ArticleSrvice>();
            builder.Services.AddTransient<ISourceService, SourceService>();
            builder.Services.AddTransient<ICommentService, CommentService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IRoleService, RoleService>();

            builder.Services.AddAutoMapper(typeof(Program));


            builder.Services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            // Add the processing server as IHostedService
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JWTToken_Auth_API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                        new OpenApiSecurityScheme 
                        {
                            Reference = new OpenApiReference 
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseHangfireDashboard();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.MapHangfireDashboard();

            app.Run();
        }
    }
}