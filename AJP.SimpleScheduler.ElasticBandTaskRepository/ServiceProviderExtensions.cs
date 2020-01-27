using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AJP.SimpleScheduler.ElasticBandTaskRepository
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddElasticBandScheduledTaskRepository(this IServiceCollection services, bool UseElasticBandRepository = true)
        {
            //services.AddSingleton<IScheduledTaskRepository, ElasticBandScheduledTaskRepository>();
            services.Replace(ServiceDescriptor.Singleton<IScheduledTaskRepository, ElasticBandScheduledTaskRepository>());
            services.AddSingleton<ElasticScheduledTaskRepository<ScheduledTask>>();

            return services;
        }
    }
}
