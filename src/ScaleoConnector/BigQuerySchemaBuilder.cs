using Google.Cloud.BigQuery.V2;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaleoConnector
{
    // BigQuerySchemaBuilder is responsible for building a BigQuery table schema
    // based on the structure of JSON data.
    public static class BigQuerySchemaBuilder
    {
        // BuildFromJson analyzes the first JSON row and generates a BigQuery schema.
        // Parameters:
        //   rows - list of JSON elements representing data rows
        // Returns:
        //   A TableSchema object that matches the JSON structure.
        public static TableSchema BuildFromJson(List<JsonElement> rows)
        {
            var schema = new TableSchemaBuilder();

            // If no rows are provided, return an empty schema.
            if (rows.Count == 0) return schema.Build();

            // Use the first row to infer the schema (assumes all rows have similar structure).
            var first = rows[0];

            // Iterate through all properties of the JSON object.
            foreach (var prop in first.EnumerateObject())
            {
                var name = prop.Name;                 // Property name becomes column name
                var type = BigQueryDbType.String;     // Default type is String

                // Determine column type based on JSON value kind.
                switch (prop.Value.ValueKind)
                {
                    case JsonValueKind.Number:
                        type = BigQueryDbType.Numeric; // Numbers → Numeric
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        type = BigQueryDbType.Bool;    // Boolean values → Bool
                        break;
                    case JsonValueKind.String:
                        // Attempt to parse string as a DateTime.
                        if (DateTime.TryParse(prop.Value.GetString(), out _))
                            type = BigQueryDbType.Timestamp; // If parse succeeds → Timestamp
                        else
                            type = BigQueryDbType.String;    // Otherwise → String
                        break;
                    default:
                        type = BigQueryDbType.String;        // Fallback → String
                        break;
                }

                // Add column definition to schema.
                schema.Add(name, type);
            }

            // Build and return the final schema.
            return schema.Build();
        }
    }
}