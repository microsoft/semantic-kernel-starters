using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

/// <summary>
/// This is the main application service.
/// This takes console input, then sends it to the configured AI service, and then prints the response.
/// All conversation history is maintained in the chat history.
/// </summary>
internal class ConsoleChat : IHostedService
{
    private readonly Kernel _kernel;
    private readonly IHostApplicationLifetime _lifeTime;

    public ConsoleChat(Kernel kernel, IHostApplicationLifetime lifeTime)
    {
        this._kernel = kernel;
        this._lifeTime = lifeTime;
    }

    /// <summary>
    /// Start the service.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => this.ExecuteAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop a running service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// The main execution loop. It will use any of the available plugins to perform actions
    /// </summary>
    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        ChatHistory chatMessages = [];
        IChatCompletionService chatCompletionService = this._kernel.GetService<IChatCompletionService>();

        // Loop till we are cancelled
        while (!cancellationToken.IsCancellationRequested)
        {
            // Get user input
            System.Console.Write("User > ");
            chatMessages.AddUserMessage(Console.ReadLine()!);

            // Get the chat completions
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionCallBehavior = FunctionCallBehavior.AutoInvokeKernelFunctions
            };
            IAsyncEnumerable<StreamingChatMessageContent> result =
                chatCompletionService.GetStreamingChatMessageContentsAsync(
                    chatMessages,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: this._kernel,
                    cancellationToken: cancellationToken);

            // Print the chat completions
            ChatMessageContent? chatMessageContent = null;
            await foreach (var content in result)
            {
                if (content.Role.HasValue)
                {
                    System.Console.Write("Assistant > ");
                    chatMessageContent = new(
                        content.Role ?? AuthorRole.Assistant,
                        content.ModelId!,
                        content.Content!,
                        content.InnerContent,
                        content.Encoding,
                        content.Metadata
                    );
                }
                System.Console.Write(content.Content);
                chatMessageContent!.Content += content.Content;
            }
            System.Console.WriteLine();
            chatMessages.AddMessage(chatMessageContent!);
        }
    }
}
