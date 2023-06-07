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
    private readonly IChatCompletion chatCompletion;
    private readonly ChatHistory chatHistory;
    
    public ChatSkill(IKernel kernel, KernelSettings kernelSettings)
    {
        // Set up the chat completion and history - the history is used to keep track of the conversation
        // and is part of the prompt sent to ChatGPT to allow a continuous conversation
        this.chatCompletion = kernel.GetService<IChatCompletion>();
        this.chatHistory = this.chatCompletion.CreateNewChat(kernelSettings.SystemPrompt);
    }

    /// <summary>
    /// Send a prompt to the LLM.
    /// </summary>
    [SKFunction("Send a prompt to the LLM.")]
    [SKFunctionName("Prompt")]
    public async Task<string> Prompt(string prompt)
    {
        var reply = string.Empty;
        try
        {
            // Add the question as a user message to the chat history, then send everything to OpenAI.
            // The chat history is used as context for the prompt
            this.chatHistory.AddMessage(ChatHistory.AuthorRoles.User, prompt);
            reply = await this.chatCompletion.GenerateMessageAsync(this.chatHistory);

            // Add the interaction to the chat history.
            this.chatHistory.AddMessage(ChatHistory.AuthorRoles.Assistant, reply);
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
        foreach (var message in this.chatHistory.Messages)
        {
            // Depending on the role, use a different color
            var role = "None:      ";
            switch (message.AuthorRole)
            {
                case ChatHistory.AuthorRoles.System:
                    role = "System:    ";
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case ChatHistory.AuthorRoles.User:
                    role = "User:      ";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case ChatHistory.AuthorRoles.Assistant:
                    role = "Assistant: ";
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            // Write the role and the message
            Console.WriteLine($"{role}{message.Content}");
        }

        return Task.CompletedTask;
    }
}