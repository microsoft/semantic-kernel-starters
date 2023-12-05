using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;
using Microsoft.SemanticKernel.PromptTemplate.Handlebars;
using Plugins;

var kernelSettings = KernelSettings.LoadSettings();

using var loggerFactory = LoggerFactory.Create(b =>
    {
        b.SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
            .AddConsole()
            .AddDebug();
    });

Kernel kernel = new KernelBuilder()
    .WithLoggerFactory(loggerFactory)
    .WithChatCompletionService(kernelSettings)
    .WithPlugins(plugins => plugins.AddPluginFromObject<LightPlugin>())
    .Build();


// Load prompt from resource
using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream("prompts.Chat.yaml")!);
KernelFunction prompt = kernel.CreateFunctionFromPromptYaml(
    reader.ReadToEnd(),
    promptTemplateFactory: new HandlebarsPromptTemplateFactory()
);

ChatHistory chatMessages = [];
IChatCompletionService chatCompletionService = kernel.GetService<IChatCompletionService>();

// Loop till we are cancelled
while (true)
{
    // Get user input
    System.Console.Write("User > ");
    chatMessages.AddUserMessage(Console.ReadLine()!);

    // Get the chat completions
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
    };

    var result = kernel.InvokeStreamingAsync<StreamingChatMessageContent>(
        prompt,
        arguments: new(openAIPromptExecutionSettings) {
            { "messages", chatMessages }
        });

    // Print the chat completions
    ChatMessageContent? chatMessageContent = null;
    await foreach (var content in result)
    {
        System.Console.Write(content);
        if (chatMessageContent == null)
        {
            System.Console.Write("Assistant > ");
            chatMessageContent = new ChatMessageContent(
                content.Role ?? AuthorRole.Assistant,
                content.Content!,
                content.InnerContent,
                content.Encoding,
                content.Metadata);
        }
        else
        {
            chatMessageContent.Content += content;
        }
    }
    System.Console.WriteLine();
    chatMessages.AddMessage(chatMessageContent!);
}
