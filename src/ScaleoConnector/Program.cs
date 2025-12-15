using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ScaleoConnector
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Create a generic Host builder with default settings (logging, configuration, DI).
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, cfg) =>
                {
                    // Load configuration from appsettings.json (optional, reload on change).
                    cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    // Also load configuration from environment variables.
                    cfg.AddEnvironmentVariables();
                })
                .ConfigureServices((ctx, services) =>
                {
                    // Register ScaleoClient as a singleton service (one instance for the whole app).
                    services.AddSingleton<ScaleoClient>();

                    // Register BigQueryClientWrapper as a singleton service.
                    services.AddSingleton<BigQueryClientWrapper>();

                    // Register ConnectorWorker as a hosted background service.
                    services.AddHostedService<ConnectorWorker>();
                })
                .Build();

            // Run the host (starts the application and hosted services).
            await host.RunAsync();
        }
    }
}