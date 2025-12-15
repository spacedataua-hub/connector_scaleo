using Google.Cloud.BigQuery.V2;
using System.Collections.Generic;
using System.Text.Json;

namespace ScaleoConnector
{
    // BigQueryRowBuilder is responsible for converting JSON rows into BigQueryInsertRow objects.
    public static class BigQueryRowBuilder
    {
        // Build transforms a list of JsonElement rows into a collection of BigQueryInsertRow.
        // Parameters:
        //   rows - list of JSON elements representing data rows
        // Returns:
        //   IEnumerable<BigQueryInsertRow> that can be inserted into BigQuery.
        public static IEnumerable<BigQueryInsertRow> Build(List<JsonElement> rows)
        {
            foreach (var r in rows)
            {
                var row = new BigQueryInsertRow();

                // Iterate through all properties of the JSON object.
                foreach (var prop in r.EnumerateObject())
                {
                    switch (prop.Value.ValueKind)
                    {
                        case JsonValueKind.String:
                            // Add string values directly.
                            row.Add(prop.Name, prop.Value.GetString());
                            break;

                        case JsonValueKind.Number:
                            // Try to parse number as integer first, then as double.
                            if (prop.Value.TryGetInt64(out var i)) row.Add(prop.Name, i);
                            else if (prop.Value.TryGetDouble(out var d)) row.Add(prop.Name, d);
                            break;

                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            // Add boolean values.
                            row.Add(prop.Name, prop.Value.GetBoolean());
                            break;

                        default:
                            // For other types (objects, arrays, null), store as string representation.
                            row.Add(prop.Name, prop.Value.ToString());
                            break;
                    }
                }

                // Yield the constructed row for BigQuery insertion.
                yield return row;
            }
        }
    }
}