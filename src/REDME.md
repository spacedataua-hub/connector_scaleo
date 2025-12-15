
# ScaleoConnector (.NET)

## 📌 Description
ScaleoConnector is a .NET application designed to integrate with the **Scaleo** platform and transfer data to **Google BigQuery**.  
The application is built on `Generic Host`, uses Dependency Injection, configuration via `appsettings.json`, and runs as a background service. Its main purpose is to automatically fetch reports from Scaleo, validate their quality, and load them into BigQuery.

## 🚀 Features
- Connects to the Scaleo API and retrieves reports for a specified period
- Validates data quality (validity, uniqueness of `click_id`)
- Automatically creates a BigQuery table based on JSON structure
- Converts JSON data into rows for insertion
- Inserts data into BigQuery
- Logs process and errors
- Supports Docker for containerization

## 📂 Project Structure
```
ScaleoConnector/
├── docker/                  # containerization
├── logs/                    # logs
├── scripts/
│   └── build.ps1            # build script
├── src/ScaleoConnector/
│   ├── BigQueryClientWrapper.cs   # BigQuery wrapper
│   ├── BigQueryRowBuilder.cs      # JSON → row conversion
│   ├── BigQuerySchemaBuilder.cs   # schema builder
│   ├── ConnectorWorker.cs         # background service
│   ├── DataQualityChecker.cs      # data validation
│   ├── ScaleoClient.cs            # Scaleo API client
│   ├── Program.cs                 # entry point
│   └── ScaleoConnector.csproj
├── tests/                  # unit tests
├── appsettings.json        # configuration
├── Dockerfile              # containerization
├── .gitignore
└── ConnectorSolution.sln   # solution file
```

 ⚙️ Requirements
- .NET 6.0+
- NuGet packages:
  - `Microsoft.Extensions.Hosting`
  - `Microsoft.Extensions.Configuration`
  - `Google.Cloud.BigQuery.V2`
  - `Newtonsoft.Json`



## 🧩 Main Components
- **ScaleoClient** — client for Scaleo API, retrieves reports for a given period.  
- **DataQualityChecker** — validates data, removes empty or duplicate `click_id`.  
- **BigQuerySchemaBuilder** — builds table schema from JSON.  
- **BigQueryRowBuilder** — converts JSON objects into rows for insertion.  
- **BigQueryClientWrapper** — manages BigQuery connection, creates table, inserts data.  
- **ConnectorWorker** — background service that orchestrates the process.  
- **Program.cs** — entry point, configures Host and registers services.


