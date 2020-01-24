using System;
using System.IO;
using AJP.SimpleScheduler.DateTimeProvider;
using AJP.SimpleScheduler.TaskExecution;
using AJP.SimpleScheduler.TimerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleScheduler;

namespace AJP.SimpleScheduler.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var config = LoadConfiguration();

            var services = ConfigureServices(config); 
            
            var serviceProvider = services.BuildServiceProvider();            

            serviceProvider.GetService<App>().Run();
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 
            return builder.Build();
        }

        private static IServiceCollection ConfigureServices(IConfiguration config)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(config);
            services.AddSingleton<IDateTimeProvider, DateTimeProvider.DateTimeProvider>(); 
            services.AddSingleton<ISchedulerState, SchedulerState>();              
            services.AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue());
            services.AddSingleton<ITimerService, TimerService.TimerService>();
            services.AddSingleton<INormalJobExecuter, NormalJobExecuter>(); 
            services.AddSingleton<App>();
            return services;
        }
    }

    public class App 
    {
        private readonly ISchedulerState _schedulerState;
        private readonly ITimerService _timerService;
        private readonly INormalJobExecuter _normalJobExecuter;

        public App(ISchedulerState schedulerState, ITimerService timerService, INormalJobExecuter normalJobExecuter)
        {
            _schedulerState = schedulerState;
            _timerService = timerService;
            _normalJobExecuter = normalJobExecuter;
        }

        public void Run() 
        {
            _schedulerState.AddScheduledTask(new SimpleSchedule().Run("hello andrew").Now());

            _timerService.Start();

            while(true) 
            {
                // do nothing
            }
        }
    }
}
