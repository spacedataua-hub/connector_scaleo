using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScaleoConnector
{
    // ConnectorWorker is a background service that runs continuously when the application starts.
    public class ConnectorWorker : BackgroundService
    {
        private readonly ILogger<ConnectorWorker> _log;   // Logger for diagnostic messages
        private readonly ScaleoClient _scaleo;            // Client for fetching data from Scaleo API
        private readonly BigQueryClientWrapper _bq;       // Client wrapper for inserting data into BigQuery

        // Constructor: dependencies are injected via DI (logger, ScaleoClient, BigQueryClientWrapper).
        public ConnectorWorker(ILogger<ConnectorWorker> log, ScaleoClient scaleo, BigQueryClientWrapper bq)
        {
            _log = log;
            _scaleo = scaleo;
            _bq = bq;
        }

        // ExecuteAsync is the main entry point for the background service.
        // It runs when the host starts and continues until the application shuts down.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _log.LogInformation("Connector started");
            try
            {
                // Define the time range for fetching reports (last 10 days).
                var from = DateTime.UtcNow.AddDays(-10);
                var to = DateTime.UtcNow;

                // Fetch reports from Scaleo API.
                var rows = await _scaleo.FetchReportsAsync(from, to, stoppingToken);

                // Validate data quality (remove invalid or duplicate rows).
                var valid = DataQualityChecker.Validate(rows, _log);

                // Ensure the BigQuery table exists and insert validated data.
                await _bq.EnsureTableAndInsertAsync(valid, stoppingToken);

                _log.LogInformation("Done");
            }
            catch (Exception ex)
            {
                // Log any errors that occur during execution.
                _log.LogError(ex, "Error in connector");
            }
        }
    }
}