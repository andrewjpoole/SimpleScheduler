using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using NodaTime.Extensions;

namespace AJP.SimpleScheduler
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddSimpleScheduler(this IServiceCollection services, bool addInMemoryScheduledTaskRepository = false)
        {
            services
                .AddSingleton<IClock>(SystemClock.Instance.InZone(DateTimeZoneProviders.Tzdb["Europe/London"]))
                .AddSingleton<IScheduledTaskBuilderFactory, ScheduledTaskBuilderFactory>()
                .AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue())
                .AddHostedService<TimerService.TimerService>();
            
                if(addInMemoryScheduledTaskRepository)
                    services.AddSingleton<IScheduledTaskRepository, InMemoryScheduledTaskRepository>();

            return services;
        }
    }
}
