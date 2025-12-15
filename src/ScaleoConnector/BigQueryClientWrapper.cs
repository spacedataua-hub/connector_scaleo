using Google.Cloud.BigQuery.V2;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ScaleoConnector
{
    // BigQueryClientWrapper provides a simplified interface for interacting with Google BigQuery.
    public class BigQueryClientWrapper
    {
        private readonly BigQueryClient _client;   // BigQuery client instance
        private readonly string _projectId;        // Google Cloud project ID
        private readonly string _datasetId;        // BigQuery dataset ID
        private readonly string _tableId = "scaleo_reports"; // Target table name

        // Constructor: initializes BigQuery client using environment variables.
        public BigQueryClientWrapper()
        {
            // Read project and dataset IDs from environment variables.
            // If not set, fallback values are used.
            _projectId = Environment.GetEnvironmentVariable("BQ_PROJECT") ?? "your-project-id";
            _datasetId = Environment.GetEnvironmentVariable("BQ_DATASET") ?? "your_dataset";

            // Create BigQuery client for the specified project.
            _client = BigQueryClient.Create(_projectId);
        }

        // EnsureTableAndInsertAsync checks if the table exists, creates it if necessary,
        // and inserts rows into BigQuery.
        // Parameters:
        //   rows - list of JSON elements to insert
        //   ct   - cancellation token
        public async Task EnsureTableAndInsertAsync(List<JsonElement> rows, CancellationToken ct)
        {
            // Get or create dataset reference.
            var dataset = _client.GetOrCreateDataset(_datasetId);
            var tableRef = dataset.GetTableReference(_tableId);

            // If table does not exist, build schema from JSON and create it.
            if (!_client.GetTable(dataset.Reference.ProjectId, dataset.Reference.DatasetId, _tableId).Exists())
            {
                var schema = BigQuerySchemaBuilder.BuildFromJson(rows);
                _client.CreateTable(dataset.Reference.DatasetId, _tableId, schema);
            }

            // Convert JSON rows into BigQueryInsertRow objects.
            var insertRows = BigQueryRowBuilder.Build(rows);

            // Insert rows into BigQuery table.
            await _client.InsertRowsAsync(dataset.Reference.DatasetId, _tableId, insertRows, cancellationToken: ct);
        }
    }
}