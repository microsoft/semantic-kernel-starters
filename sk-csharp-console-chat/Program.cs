using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Plugins;

// Load the kernel settings
var kernelSettings = KernelSettings.LoadSettings();

// Create the host builder with logging configured from the kernel settings.
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning);
    });

// Configure the services for the host
builder.ConfigureServices((context, services) =>
{

    // Add kernel settings to the host builder
    services
        .AddSingleton<KernelSettings>(kernelSettings)
        .AddSingleton<Kernel>(sp => new KernelBuilder()
            .WithLoggerFactory(LoggerFactory.Create(b =>
            {
                b.SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
                    .AddConsole()
                    .AddDebug();
            }))
            .WithChatCompletionService(kernelSettings)
            .WithPlugins(plugins => plugins.AddPluginFromObject<LightPlugin>())
            .Build()
        )
        .AddHostedService<ConsoleChat>();
});

// Build and run the host. This keeps the app running using the HostedService.
var host = builder.Build();
await host.RunAsync();
