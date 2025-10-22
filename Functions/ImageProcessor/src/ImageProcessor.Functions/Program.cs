using Azure.Storage.Blobs;
using ImageProcessor.Functions.Options;
using ImageProcessor.Functions.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

var builder = FunctionsApplication.CreateBuilder(args);
// Ensure local.settings.json is loaded
builder.Configuration.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false);

// Configure options using the options pattern
builder.Services.AddOptions<ProcessorOptions>()
    .Configure<IConfiguration>((settings, config) =>
    {
        settings.AzureWebJobsStorage = config["AzureWebJobsStorage"] ?? "";
        settings.MediaContainer = config["MEDIA_CONTAINER"] ?? "media";
        settings.CatalogBaseURL = config["CATALOG_BASE_URL"] ?? "";
        settings.InternalAPIKEY = config["INTERNAL_API_KEY"];

        if (string.IsNullOrWhiteSpace(settings.AzureWebJobsStorage))
            throw new InvalidOperationException("AzureWebJobsStorage is not configured.");
    });

// Core clients - now we need to resolve IOptions or use a factory
builder.Services.AddSingleton(provider =>
{
    var options = provider.GetRequiredService<IOptions<ProcessorOptions>>().Value;
    return new BlobServiceClient(options.AzureWebJobsStorage);
});
builder.Services.AddHttpClient();

//  Application services
builder.Services.AddSingleton<IImageJobDeserializer, JsonImageJobDeserializer>();
builder.Services.AddSingleton<IThumbnailService, ImageSharpThumbnailService>();
builder.Services.AddSingleton<ICatalogInternalClient, CatalogInternalClient>();

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
