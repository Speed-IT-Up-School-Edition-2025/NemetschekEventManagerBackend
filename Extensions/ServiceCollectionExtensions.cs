using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using System.Reflection;

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
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Nemetschek Event API",
                    Version = "v1",
                    Description = "This API stores and manages data for the Nemetschek Event Manager application.",
                    Contact = new OpenApiContact
                    {
                        Name = "Mihail Tenev",
                        Email = "mtenev@outlook.com",
                    }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            return services;
        }
    }
}
