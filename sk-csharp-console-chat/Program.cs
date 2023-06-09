using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using Skills;

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
    // Create a logger factory with the log level from the kernel settings.
    using var loggerFactory = LoggerFactory.Create(b =>
    {
        b.SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
            .AddConsole()
            .AddDebug();
    });

    // Create a Semantic Kernel using our logger and kernel settings.
    var kernel = new KernelBuilder()
        .WithLogger(loggerFactory.CreateLogger<IKernel>())
        .WithCompletionService(kernelSettings)
        .Build();

    // Add Semantic Kernel to the host builder
    services.AddSingleton<IKernel>(kernel);

    // Add kernel settings to the host builder
    services.AddSingleton<KernelSettings>(kernelSettings);

    // Add Native Skills to the host builder
    services.AddSingleton<ConsoleSkill>();
    services.AddSingleton<ChatSkill>();

    // Add the primary hosted service to the host builder to start the loop.
    services.AddHostedService<ConsoleGPTService>();
});

// Build and run the host. This keeps the app running using the HostedService.
var host = builder.Build();
await host.RunAsync();
