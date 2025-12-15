using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace ScaleoConnector
{
    // DataQualityChecker provides validation logic for JSON rows.
    public static class DataQualityChecker
    {
        // Validate checks each JSON row for data quality issues.
        // Parameters:
        //   rows - list of JSON elements to validate
        //   log  - logger instance for warnings
        // Returns:
        //   A filtered list of valid JSON elements.
        public static List<JsonElement> Validate(List<JsonElement> rows, ILogger log)
        {
            var outRows = new List<JsonElement>();   // List to store valid rows
            var seen = new HashSet<string>();        // Track unique click_id values

            foreach (var r in rows)
            {
                // Example: check if "click_id" property exists and is a string
                if (r.TryGetProperty("click_id", out var click) && click.ValueKind == JsonValueKind.String)
                {
                    var id = click.GetString();

                    // Skip if click_id is empty
                    if (string.IsNullOrEmpty(id)) { log.LogWarning("Empty click_id, skipping"); continue; }

                    // Skip if click_id is already seen (duplicate)
                    if (seen.Contains(id)) { continue; }

                    // Add new unique click_id to the set and keep the row
                    seen.Add(id);
                    outRows.Add(r);
                }
                else
                {
                    // Log warning if click_id is missing
                    log.LogWarning("Missing click_id, skipping");
                }
            }

            // Return only valid and unique rows
            return outRows;
        }
    }
}