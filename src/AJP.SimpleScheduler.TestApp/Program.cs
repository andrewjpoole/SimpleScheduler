using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Serilog.Sinks.Elasticsearch;
using System.IO;
using AJP.ElasticBand;
using AJP.SimpleScheduler.ElasticBandTaskRepository;

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
                        .AddElasticBand()
                        .AddElasticBandScheduledTaskRepository()
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
    }
}
