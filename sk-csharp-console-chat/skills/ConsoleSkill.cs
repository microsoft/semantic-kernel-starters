using System.ComponentModel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Skills;

/// <summary>
/// A Sematic Kernel skill that provides the ability to read and write from the console
/// </summary>
internal class ConsoleSkill
{
    private bool _isGoodbye = false;

    /// <summary>
    /// Gets input from the console
    /// </summary>
    [SKFunction, Description("Get console input.")]
    public Task<string> ListenAsync(SKContext context)
    {
        return Task.Run(() =>
        {
            var line = "";

            while (string.IsNullOrWhiteSpace(line))
            {
                line = Console.ReadLine();
            }

            if (line.ToLower().StartsWith("goodbye"))
            {
                this._isGoodbye = true;
            }

            return line;
        });
    }

    /// <summary>
    /// Writes output to the console
    /// </summary>
    [SKFunction, Description("Write a response to the console.")]
    public Task<string> RespondAsync(string message, SKContext context)
    {
        return Task.Run(() =>
        {
            this.WriteAIResponse(message);
            return message;
        });
    }

    /// <summary>
    /// Checks if the user said goodbye
    /// </summary>
    [SKFunction, Description("Did the user say goodbye.")]
    public Task<string> IsGoodbyeAsync(SKContext context)
    {
        return Task.FromResult(this._isGoodbye ? "true" : "false");
    }

    /// <summary>
    /// Write a response to the console in green.
    /// </summary>
    private void WriteAIResponse(string response)
    {
        // Write the response in Green, then revert the console color
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(response);
        Console.ForegroundColor = oldColor;
    }
}
