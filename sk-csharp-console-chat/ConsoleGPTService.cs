using Skills;

using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

/// <summary>
/// This is the main application service.
/// This takes console input, then sends it to the configured AI service, and then prints the response.
/// All conversation history is maintained in the chat history.
/// </summary>
internal class ConsoleGPTService : IHostedService
{
    private readonly IKernel semanticKernel;
    private readonly IDictionary<string, ISKFunction> consoleSkill;
    private readonly IDictionary<string, ISKFunction> chatSkill;
    private readonly IHostApplicationLifetime lifeTime;

    public ConsoleGPTService(IKernel semanticKernel,
                             ConsoleSkill consoleSkill,
                             ChatSkill chatSkill,
                             IHostApplicationLifetime lifeTime)
    {
        this.semanticKernel = semanticKernel;
        this.lifeTime = lifeTime;

        // Import the skills to load the semantic kernel functions
        this.consoleSkill = this.semanticKernel.ImportSkill(consoleSkill);
        this.chatSkill = this.semanticKernel.ImportSkill(chatSkill);
    }

    /// <summary>
    /// Start the service.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ExecuteAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stop a running service.
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// The main execution loop. This awaits input and responds to it using semantic kernel functions.
    /// </summary>
    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Write to the console that the conversation is beginning
        await this.semanticKernel.RunAsync("Hello. Ask me a question or say goodbye to exit.", this.consoleSkill["Respond"]);

        // Loop till we are cancelled
        while (!cancellationToken.IsCancellationRequested)
        {
            // Create our pipeline
            ISKFunction[] pipeline = {this.consoleSkill["Listen"], this.chatSkill["Prompt"], this.consoleSkill["Respond"]};

            // Run the pipeline
            await this.semanticKernel.RunAsync(pipeline);
            
            // Did we say goodbye? If so, exit
            var goodbyeContext = await this.semanticKernel.RunAsync(this.consoleSkill["IsGoodbye"]);
            var isGoodbye = bool.Parse(goodbyeContext.Result);

            // If the user says goodbye, end the chat
            if (isGoodbye)
            {
                // Log the history so we can see the prompts used
                await this.semanticKernel.RunAsync(this.chatSkill["LogChatHistory"]);

                // Stop the application
                this.lifeTime.StopApplication();
                break;
            }
        }
    }
}
