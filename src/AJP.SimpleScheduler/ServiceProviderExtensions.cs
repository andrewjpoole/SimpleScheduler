using AJP.ElasticBand;
using AJP.SimpleScheduler.DateTimeProvider;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AJP.SimpleScheduler
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddSimpleScheduler(this IServiceCollection services, bool UseElasticBandRepository = true)
        {
            services
                .AddSingleton<IDateTimeProvider, DateTimeProvider.DateTimeProvider>()
                .AddSingleton<IScheduledTaskBuilderFactory, ScheduledTaskBuilderFactory>()
                .AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue())
                .AddSingleton<INormalJobExecuter, NormalJobExecuter>()
                .AddHostedService<TimerService.TimerService>();

            if (UseElasticBandRepository)
            { 
                services.AddSingleton<IScheduledTaskRepository, ElasticBandScheduledTaskRepository>();
                services.AddSingleton<ElasticScheduledTaskRepository<ScheduledTask>>();
            }
            else
            { 
                services.AddSingleton<IScheduledTaskRepository, InMemoryScheduledTaskRepository>(); 
            }

            return services;
        }

        public static IServiceCollection AddElasticBand(this IServiceCollection services)
        {
            services
                .AddHttpClient()
                .AddSingleton<IElasticQueryBuilder, ElasticQueryBuilder>()
                .AddSingleton<IElasticBand, ElasticBand.ElasticBand>();

            return services;
        }
    }
}
