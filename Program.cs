using System.Text.Json.Serialization;
using CodeCrewShop.Data;
using CodeCrewShop.Models.Product;
using CodeCrewShop.Repositories.Implementations;
using CodeCrewShop.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CodeCrewShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔗 Add DbContext with SQL Server
            builder.Services.AddDbContext<CodeCrewShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CodeCrewShopContext")
                    ?? throw new InvalidOperationException("Connection string 'CodeCrewShopContext' not found.")));

            // 🧩 Add Controllers
            builder.Services.AddControllers();

            builder.Services.AddControllers().AddJsonOptions(options =>
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // 🧾 Add Swagger (OpenAPI)
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo
            //    {
            //        Title = "CodeCrewShop API",
            //        Version = "v1",
            //        Description = "API documentation for CodeCrewShop"
            //    });
            //});

            // 🔹 Add Authentication and set JWT as default scheme
            var config = builder.Configuration;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            
            // Swagger JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "CodeCrewShop API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT with Bearer prefix. Example: Bearer {token}",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });

            // 🧰 Register Repositories and Unit of Work
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped(typeof(IProductRepository<>), typeof(ProductRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


            var app = builder.Build();

            // 🌐 Enable Swagger middleware in all environments (or only Development)
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeCrewShop API v1");
                c.RoutePrefix = "swagger"; // so you can access at /swagger
            });

            // 🛡️ Enable HTTPS and Authorization
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // 🧭 Map Controllers
            app.MapControllers();

            app.Run();
        }
    }
}
