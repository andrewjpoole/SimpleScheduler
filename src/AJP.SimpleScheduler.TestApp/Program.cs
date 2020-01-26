using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using AJP.SimpleScheduler.DateTimeProvider;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using AJP.SimpleScheduler.TimerService;
using Serilog.Sinks.Elasticsearch;
using System.IO;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.ElasticBand;

namespace AJP.SimpleScheduler.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SimpleScheduler test app starting...");

            SetupStaticLogger();

            var config = LoadConfiguration();
            CreateHostBuilder(args, config).Build().Run();
        }

        private static void SetupStaticLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    IndexFormat = "logstash-{0:yyyy.MM}",
                    BufferBaseFilename = $"C:\\Temp\\Logs\\SerilogElasticBuffer",
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                }).CreateLogger();
        }

        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(config)

                        //.AddHttpClient()
                        //.AddSingleton<IElasticQueryBuilder, ElasticQueryBuilder>()
                        //.AddSingleton<IElasticBand, ElasticBand.ElasticBand>()

                        .AddElasticBand()

                        //.AddSingleton<IDateTimeProvider, DateTimeProvider.DateTimeProvider>()
                        //.AddSingleton<IScheduledTaskBuilderFactory, ScheduledTaskBuilderFactory>()
                        //.AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue())
                        //.AddSingleton<INormalJobExecuter, NormalJobExecuter>()
                        //.AddHostedService<TimerService.TimerService>()
                        //.AddSingleton<IScheduledTaskRepository, InMemoryScheduledTaskRepository>()
                        //.AddSingleton<IScheduledTaskRepository, ElasticBandScheduledTaskRepository>()

                        .AddSimpleScheduler()

                        .AddHostedService<App>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.AddSerilog();
                }
            );
    

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true); 
            return builder.Build();
        }

        //private static IServiceCollection ConfigureServices(IConfiguration config)
        //{
        //    IServiceCollection services = new ServiceCollection();
        //    services.AddSingleton(config);
        //    services.AddSingleton<IDateTimeProvider, DateTimeProvider.DateTimeProvider>(); 
        //    services.AddSingleton<IScheduledTaskRepository, InMemoryScheduledTaskRepository>();              
        //    services.AddSingleton<IDueTaskJobQueue>(new DueTaskJobQueue());
        //    services.AddSingleton<ITimerService, TimerService.TimerService>();
        //    services.AddSingleton<INormalJobExecuter, NormalJobExecuter>(); 
        //    services.AddSingleton<App>();
        //    return services;
        //}
    }
}
