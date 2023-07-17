using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Skills;

/// <summary>
/// A Sematic Kernel skill that interacts with ChatGPT
/// </summary>
internal class ChatSkill
{
    private readonly IChatCompletion _chatCompletion;
    private readonly ChatHistory _chatHistory;

    private readonly Dictionary<AuthorRole, string> _roleToDisplayRole = new()
        {
            {AuthorRole.System, "System:    "},
            {AuthorRole.User, "User:      "},
            {AuthorRole.Assistant, "Assistant: "}
        };
    private readonly Dictionary<AuthorRole, ConsoleColor> _roleToConsoleColor = new()
        {
            {AuthorRole.System, ConsoleColor.Blue},
            {AuthorRole.User, ConsoleColor.Yellow},
            {AuthorRole.Assistant, ConsoleColor.Green}
        };

    public ChatSkill(IKernel kernel, KernelSettings kernelSettings)
    {
        // Set up the chat completion and history - the history is used to keep track of the conversation
        // and is part of the prompt sent to ChatGPT to allow a continuous conversation
        this._chatCompletion = kernel.GetService<IChatCompletion>();
        this._chatHistory = this._chatCompletion.CreateNewChat(kernelSettings.SystemPrompt);
    }

    /// <summary>
    /// Send a prompt to the LLM.
    /// </summary>
    [SKFunction, Description("Send a prompt to the LLM.")]
    public async Task<string> PromptAsync(string prompt)
    {
        var reply = string.Empty;
        try
        {
            // Add the question as a user message to the chat history, then send everything to OpenAI.
            // The chat history is used as context for the prompt
            this._chatHistory.AddMessage(AuthorRole.User, prompt);
            reply = await this._chatCompletion.GenerateMessageAsync(this._chatHistory);

            // Add the interaction to the chat history.
            this._chatHistory.AddMessage(AuthorRole.Assistant, reply);
        }
        catch (AIException aiex)
        {
            // Reply with the error message if there is one
            reply = $"OpenAI returned an error ({aiex.Message}). Please try again.";
        }

        return reply;
    }

    /// <summary>
    /// Log the history of the chat with the LLM.
    /// This will log the system prompt that configures the chat, along with the user and assistant messages.
    /// </summary>
    [SKFunction, Description("Log the history of the chat with the LLM.")]
    public Task LogChatHistory()
    {
        Console.WriteLine();
        Console.WriteLine("Chat history:");
        Console.WriteLine();

        // Log the chat history including system, user and assistant (AI) messages
        foreach (var message in this._chatHistory.Messages)
        {
            string role = "None:      ";
            // Depending on the role, use a different color
            if (this._roleToDisplayRole.TryGetValue(message.Role, out var displayRole))
            {
                role = displayRole;
            }

            if (this._roleToConsoleColor.TryGetValue(message.Role, out var color))
            {
                Console.ForegroundColor = color;
            }

            // Write the role and the message
            Console.WriteLine($"{role}{message.Content}");
        }

        return Task.CompletedTask;
    }
}
