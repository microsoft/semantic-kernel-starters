using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Skills;

/// <summary>
/// A Sematic Kernel skill that provides the ability to read and write from the console
/// </summary>
internal class ConsoleSkill
{
    private bool isGoodbye = false;

    /// <summary>
    /// Gets input from the console
    /// </summary>
    [SKFunction("Get console input.")]
    [SKFunctionName("Listen")]
    public Task<string> Listen(SKContext context)
    {
        return Task.Run(() => {
            var line = "";

            while (string.IsNullOrWhiteSpace(line))
            {
                line = Console.ReadLine();
            }

            if (line.ToLower().StartsWith("goodbye"))
                this.isGoodbye = true;

            return line;
        });
    }

    /// <summary>
    /// Writes output to the console
    /// </summary>
    [SKFunction("Write a response to the console.")]
    [SKFunctionName("Respond")]
    public Task<string> Respond(string message, SKContext context)
    {
        return Task.Run(() => {
            WriteAIResponse(message);
            return message;
        });
    }

    /// <summary>
    /// Checks if the user said goodbye
    /// </summary>
    [SKFunction("Did the user say goodbye.")]
    [SKFunctionName("IsGoodbye")]
    public Task<string> IsGoodbye(SKContext context)
    {
        return Task.FromResult(this.isGoodbye ? "true" : "false");
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