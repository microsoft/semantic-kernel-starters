using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Filters;
using Steps;

namespace ProductDocumentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Build configuration
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        // The following code shows how to initialize and execute a process using:
        // 1. Imperative approach, by manually defining steps and configuring the relationship between them.
        // 2. Declarate approach, by importing YAML file.
        await ImperativeProcessAsync(configuration);
        await DeclarativeProcessAsync(configuration);
    }

    private static async Task ImperativeProcessAsync(IConfiguration configuration)
    {
        Console.WriteLine("\n=== Imperative Process ===\n");

        const string StartEvent = "input_message_received";

        // Create process
        ProcessBuilder process = new("ProductDocumentation");

        // Build steps
        ProcessStepBuilder getProductInfoStep = process.AddStepFromType<GetProductInfoStep>();
        ProcessStepBuilder generateDocumentationStep = process.AddStepFromType<GenerateDocumentationStep>();
        ProcessStepBuilder publishDocumentationStep = process.AddStepFromType<PublishDocumentationStep>();

        // Define relationship between steps
        process
            .OnInputEvent(StartEvent)
            .SendEventTo(new ProcessFunctionTargetBuilder(getProductInfoStep));

        getProductInfoStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(generateDocumentationStep));

        generateDocumentationStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(publishDocumentationStep));

        publishDocumentationStep
            .OnFunctionResult()
            .StopProcess();

        // Create kernel and build process from defined steps.
        Kernel kernel = CreateKernel(configuration);
        KernelProcess kernelProcess = process.Build();

        // Start process
        await using var runningProcess = await kernelProcess!.StartAsync(kernel, new() { Id = StartEvent });
    }

    private static async Task DeclarativeProcessAsync(IConfiguration configuration)
    {
        Console.WriteLine("\n=== Declarative Process ===\n");

        const string FileName = "product-documentation.process.yaml";
        const string StartEvent = "input_message_received";

        // Read Process YAML content
        string filePath = Path.Combine(AppContext.BaseDirectory, FileName);
        string content = File.ReadAllText(filePath);

        // Create kernel and load process from YAML
        Kernel kernel = CreateKernel(configuration);
        KernelProcess? kernelProcess = await ProcessBuilder.LoadFromYamlAsync(content);

        // Start process
        await using var runningProcess = await kernelProcess!.StartAsync(kernel, new() { Id = StartEvent });
    }

    private static Kernel CreateKernel(IConfiguration configuration)
    {
        string deploymentName = configuration["AZUREOPENAI_DEPLOYMENT_NAME"] ?? throw new InvalidOperationException("User secret AZUREOPENAI_DEPLOYMENT_NAME is not configured.");
        string endpoint = configuration["AZUREOPENAI_ENDPOINT"] ?? throw new InvalidOperationException("User secret AZUREOPENAI_ENDPOINT is not configured.");

        // Create kernel
        Kernel kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(deploymentName, endpoint, new AzureCliCredential())
            .Build();

        // Register console output filter
        kernel.FunctionInvocationFilters.Add(new ConsoleOutputFunctionInvocationFilter());

        return kernel;
    }
}
