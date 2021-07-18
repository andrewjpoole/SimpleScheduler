using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Serilog.Sinks.Elasticsearch;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AJP.ElasticBand;
using AJP.SimpleScheduler.ElasticBandTaskRepository;
using AJP.SimpleScheduler.Intervals;
using AJP.SimpleScheduler.ScheduledTasks;
using AJP.SimpleScheduler.ScheduledTaskStorage;
using AJP.SimpleScheduler.TaskExecution;
using Microsoft.Extensions.Logging;

namespace AJP.SimpleScheduler.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SimpleScheduler test app starting...");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var config = LoadConfiguration();
            CreateHostBuilder(args, config).Build().Run();
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddSingleton(config)
                        //.AddElasticBand()
                        //.AddElasticBandScheduledTaskRepository()
                        .AddSimpleScheduler(false)
                        .AddSingleton<IScheduledTaskRepository, LocalJsonFileScheduledTaskRepository>()
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
    }

    public class App : IHostedService
    {
        private readonly IScheduledTaskRepository _taskRepository;
        private readonly IScheduledTaskBuilderFactory _taskBuilderFactory;
        private readonly ILogger<App> _logger;

        public App(IScheduledTaskRepository scheduledTaskRepository, IScheduledTaskBuilderFactory scheduledTaskBuilderFactory, ILogger<App> logger, IDueTaskJobQueue dueTaskJobQueue)
        {
            _taskRepository = scheduledTaskRepository; // where the scheduled tasks are stored
            _taskBuilderFactory = scheduledTaskBuilderFactory; // a handy builder for scheduled tasks
            _logger = logger; // gather evidence that a task has been executed

            // Setup a task handler for all tasks
            dueTaskJobQueue.RegisterHandlerForAllTasks((scheduledTask) =>
            {
                _logger.LogInformation($"{scheduledTask.JobData} {scheduledTask.Id}");
            });

            // Setup a specific task handler for tasks which have a SpecificTaskPayload as their jobdata
            dueTaskJobQueue.RegisterHandlerWhen((scheduledTask) =>
            {
                var payload = JsonSerializer.Deserialize<SpecificTaskPayload>(scheduledTask.JobData);
                _logger.LogInformation($"SpecificTaskPayload {payload.Run()} {scheduledTask.Id}");
            }, task => task.JobDataTypeName == nameof(SpecificTaskPayload));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Add some tasks to the repository
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task now!").Now());
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task after 10 seconds").After(Lapse.Seconds(30)));
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task at a specified DateTime").At(DateTime.UtcNow.AddSeconds(20)));
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task every 5 seconds for 3 times").Every(Lapse.Seconds(5), 3));
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData("* run task every 20 seconds starting at now + 1mm").EveryStartingAt(Lapse.Seconds(20), DateTime.UtcNow.AddMinutes(1)).CreateTask());

           var payload = new SpecificTaskPayload()
           {
               SomethingSpecific = "Bob"
           };
           _taskRepository.AddScheduledTask(_taskBuilderFactory.CreateBuilder().WithJobData(payload).Now().CreateTask());
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }

    public class SpecificTaskPayload
    {
        public string SomethingSpecific { get; set; }

        public string Run() => $"something specific {SomethingSpecific} @ {DateTime.UtcNow}";
    }
}
