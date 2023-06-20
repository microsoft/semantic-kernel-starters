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
    [SKFunction("Send a prompt to the LLM.")]
    [SKFunctionName("Prompt")]
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
    [SKFunction("Log the history of the chat with the LLM.")]
    [SKFunctionName("LogChatHistory")]
    public Task LogChatHistory()
    {
        Console.WriteLine();
        Console.WriteLine("Chat history:");
        Console.WriteLine();

        // Log the chat history including system, user and assistant (AI) messages
        foreach (var message in this._chatHistory.Messages)
        {
            // Depending on the role, use a different color
            var role = "None:      ";
            if (message.Role == AuthorRole.System)
            {
                role = "System:    ";
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (message.Role == AuthorRole.User)
            {
                role = "User:      ";
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            else if (message.Role == AuthorRole.Assistant)
            {
                role = "Assistant: ";
                Console.ForegroundColor = ConsoleColor.Green;
            }

            // Write the role and the message
            Console.WriteLine($"{role}{message.Content}");
        }

        return Task.CompletedTask;
    }
}
