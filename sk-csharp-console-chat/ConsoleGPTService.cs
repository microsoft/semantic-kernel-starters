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
    private readonly IKernel _semanticKernel;
    private readonly IDictionary<string, ISKFunction> _consoleSkill;
    private readonly IDictionary<string, ISKFunction> _chatSkill;
    private readonly IHostApplicationLifetime _lifeTime;

    public ConsoleGPTService(IKernel semanticKernel,
                             ConsoleSkill consoleSkill,
                             ChatSkill chatSkill,
                             IHostApplicationLifetime lifeTime)
    {
        _semanticKernel = semanticKernel;
        _lifeTime = lifeTime;

        // Import the skills to load the semantic kernel functions
        _consoleSkill = _semanticKernel.ImportSkill(consoleSkill);
        _chatSkill = _semanticKernel.ImportSkill(chatSkill);
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
        await _semanticKernel.RunAsync("Hello. Ask me a question or say goodbye to exit.", _consoleSkill["Respond"]);

        // Loop till we are cancelled
        while (!cancellationToken.IsCancellationRequested)
        {
            // Create our pipeline
            ISKFunction[] pipeline = {_consoleSkill["Listen"], _chatSkill["Prompt"], _consoleSkill["Respond"]};

            // Run the pipeline
            await _semanticKernel.RunAsync(pipeline);
            
            // Did we say goodbye? If so, exit
            var goodbyeContext = await _semanticKernel.RunAsync(_consoleSkill["IsGoodbye"]);
            var isGoodbye = bool.Parse(goodbyeContext.Result);

            // If the user says goodbye, end the chat
            if (isGoodbye)
            {
                // Log the history so we can see the prompts used
                await _semanticKernel.RunAsync(_chatSkill["LogChatHistory"]);

                // Stop the application
                _lifeTime.StopApplication();
                break;
            }
        }
    }
}
