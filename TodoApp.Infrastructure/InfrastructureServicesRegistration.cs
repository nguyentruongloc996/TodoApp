using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TodoApp.Application.Abstraction;
using TodoApp.Application.Abstraction.Repositories;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Auth;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories;
using TodoApp.Infrastructure.Services;

namespace TodoApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataProtection();

            // Database Configuration
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Identity Configuration
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT Configuration
            var jwtSection = configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            services.AddAuthorization();

            // Services
            services.AddScoped<IAuthService, AuthService>();

            // Repository Implementations
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISubTaskRepository, SubTaskRepository>();
            services.AddScoped<Persistence.Repositories.Interfaces.IApplicationUserRepository, ApplicationUserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Unit of Work Registration - Register the concrete class first
            services.AddScoped<UnitOfWork>();

            // Unit of Work and Token Provider
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());
            services.AddScoped<IInfrastructureUnitOfWork>(provider => provider.GetRequiredService<UnitOfWork>());

            services.AddScoped<ITokenProvider, TokenProvider>();

            // Identity-dependent services (stay in Infrastructure)
            services.AddScoped<IUserIdentityService, UserIdentityService>();

            // Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}