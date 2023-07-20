using Application.Services;
using MediatR.NotificationPublishers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Reflection;
using static ProtoDefinitions.MoviesApi;

namespace Application
{
    public static class DependencyInjection
    {       
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();

                config.NotificationPublisher = new TaskWhenAllPublisher();
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            IConfigurationSection redisConfig = Configuration.GetSection("Redis");
            string redisConnectionString = redisConfig["ConnectionString"];
            string redisInstanceName = redisConfig["InstanceName"];

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

            // Create Redis cache configuration
            var redisConfiguration = new RedisConfiguration()
            {
                AbortOnConnectFail = false,
                KeyPrefix = redisInstanceName,
                ConnectionString = redisConnectionString
            };

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "MyRedisInstance";
            });

            // Register Redis cache
            services.AddSingleton(redisConfiguration);
            services.AddSingleton<ISerializer, NewtonsoftSerializer>();

            IConfigurationSection movieApiConfig = Configuration.GetSection("MovieApi");
            string movieApiConnectionString = movieApiConfig["ConnectionString"];
            services.AddGrpcClient<MoviesApiClient>(options =>
            {
                options.Address = new Uri(movieApiConnectionString);
            });

            services.AddSingleton<ApiClientGrpc>();

            return services;
        }
    }
}
