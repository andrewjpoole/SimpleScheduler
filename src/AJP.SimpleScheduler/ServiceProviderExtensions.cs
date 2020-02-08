using AJP.ElasticBand;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace AJP.SimpleScheduler
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddSimpleScheduler(this IServiceCollection services)
        {
            services
                .AddSingleton<IClock>(SystemClock.Instance)
                .AddSingleton<IScheduledTaskBuilderFactory, ScheduledTaskBuilderFactory>()
                .AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue())
                .AddSingleton<INormalJobExecuter, NormalJobExecuter>()
                .AddHostedService<TimerService.TimerService>()            
                .AddSingleton<IScheduledTaskRepository, InMemoryScheduledTaskRepository>();

            return services;
        }
    }
}
