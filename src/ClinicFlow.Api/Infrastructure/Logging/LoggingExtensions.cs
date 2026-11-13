using Serilog;
using Serilog.Events;

namespace ClinicFlow.Api.Infrastructure.Logging;

public static class LoggingExtensions
{
    /// <summary>
    /// Serilog replaces the default logger. Console and File come from appsettings.json (the "Serilog" section).
    /// Azure Blob Storage is added here, only when its connection string exists:
    /// in development it comes from dotnet user-secrets, never from appsettings or the repo.
    /// </summary>
    public static WebApplicationBuilder AddSerilogLogging(this WebApplicationBuilder builder)
    {
        var azureOn = !string.IsNullOrWhiteSpace(builder.Configuration["AzureLogStorage:ConnectionString"]);
        Log.Information("Azure Blob logging: {State}", azureOn
            ? $"on (container '{builder.Configuration["AzureLogStorage:ContainerName"] ?? "logs"}')"
            : "off (no connection string in user-secrets)");

        builder.Services.AddSerilog((services, logger) =>
        {
            logger
                .ReadFrom.Configuration(builder.Configuration)   // levels, Console, File, enrichers
                .ReadFrom.Services(services);

            var blobConnection = builder.Configuration["AzureLogStorage:ConnectionString"];
            if (!string.IsNullOrWhiteSpace(blobConnection))
            {
                logger.WriteTo.AzureBlobStorage(
                    connectionString: blobConnection,
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    storageContainerName: builder.Configuration["AzureLogStorage:ContainerName"] ?? "logs",
                    storageFileName: "clinicflow/{yyyy}-{MM}-{dd}.txt",
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    period: TimeSpan.FromSeconds(5));     // logs go to Azure in small batches, every 5 seconds
            }
        });

        return builder;
    }
}
