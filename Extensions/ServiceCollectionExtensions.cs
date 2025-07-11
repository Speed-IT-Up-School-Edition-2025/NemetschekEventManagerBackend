using Microsoft.AspNetCore.Identity.UI.Services;
﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NemetschekEventManagerBackend.Models;
using System.Reflection;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Seeders;

namespace NemetschekEventManagerBackend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Add application services to the IServiceCollection
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISubmitService, SubmitService>();
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
            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<EventDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityApiEndpoints<User>();

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddCorsSupport(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
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
                    Description = "This API stores and manages data for the Nemetschek Event Manager application.\n" +
                    "\n IMPORTANT!!! : To authenticate use \"Bearer {your_access_token}\" (without the quotes and the curly brackets).",
                    Contact = new OpenApiContact
                    {
                        Name = "Mihail Tenev",
                        Email = "mtenev@outlook.com",
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
              {
                {
                  new OpenApiSecurityScheme
                  {
                    Reference = new OpenApiReference
                      {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,

                    },
                    new List<string>()
                  }
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });
            return services;
        }
        public static IServiceCollection SetupMailer(this IServiceCollection services, IConfiguration configuration)
        {
            var gmailUser = configuration["EmailSettings:GmailUser"];
            var gmailPass = configuration["EmailSettings:GmailPass"];

            services.AddTransient<IEmailSender>(_ => new GmailEmailSender(gmailUser, gmailPass));

            return services;
        }

        public static IServiceCollection Seeder(this IServiceCollection services)
        {
            services.AddHostedService<ApplicationSeeder>();
            return services;
		}
	}
}
