using System.Text.Json;
using Microsoft.SemanticKernel;

namespace Filters;

public sealed class ConsoleOutputFunctionInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"\nNode: {context.Function.PluginName}");

        await next(context);

        Console.WriteLine($"\nResult:");

        object? result = context.Result.GetValue<object>();

        if (result is string resultString)
        {
            Console.WriteLine(resultString);
        }
        else if (result is not null)
        {
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
    }
}
