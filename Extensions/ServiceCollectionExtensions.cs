using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Add application services to the IServiceCollection
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            return services;
        }
        // Add application DbContext to the IServiceCollection
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<EventDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            return services;
        }
        // Add Identity services to the IServiceCollection
        public static IServiceCollection AddAppIdentity(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<User>()
                    .AddEntityFrameworkStores<EventDbContext>();
            services.AddAuthorization();
            return services;
        }
        // Add Swagger services to the IServiceCollection
        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
