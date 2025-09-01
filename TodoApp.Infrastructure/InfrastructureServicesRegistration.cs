using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Abstraction.Services;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Persistence.Interfaces;
using TodoApp.Infrastructure.Persistence.Repositories;
using TodoApp.Infrastructure.Persistence.Repositories.Interfaces;
using TodoApp.Infrastructure.Services;

namespace TodoApp.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataProtection();

            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<ISubTaskRepository, SubTaskRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<ICacheService, RedisCacheService>();

            // Add Entity Framework
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Add Identity DbContext
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Add Redis Caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });

            return services;
        }
    }
}