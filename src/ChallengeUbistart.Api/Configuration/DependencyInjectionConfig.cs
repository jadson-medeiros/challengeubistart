using ChallengeUbistart.Api.Extensions;
using ChallengeUbistart.Business.Intefaces;
using ChallengeUbistart.Business.Notifications;
using ChallengeUbistart.Business.Services;
using ChallengeUbistart.Data.Context;
using ChallengeUbistart.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChallengeUbistart.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            // Db
            services.AddScoped<MyDbContext>();

            // Repository
            services.AddScoped<IItemRepository, ItemRepository>();

            // Services
            services.AddScoped<IItemService, ItemService>();

            // Notification
            services.AddScoped<INotify, Notify>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser, AspNetUser>();

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }
}