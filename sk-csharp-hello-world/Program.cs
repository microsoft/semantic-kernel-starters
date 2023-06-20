using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;

var kernelSettings = KernelSettings.LoadSettings();

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<IKernel>())
    .WithCompletionService(kernelSettings)
    .Build();

if (kernelSettings.EndpointType == EndpointTypes.TextCompletion)
{
    // note: using skills from the repo
    var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
    var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "FunSkill");

    var context = new ContextVariables();
    context.Set("input", "Time travel to dinosaur age");
    context.Set("style", "Wacky");

    var result = await kernel.RunAsync(context, skill["Joke"]);
    Console.WriteLine(result);
}
else if (kernelSettings.EndpointType == EndpointTypes.ChatCompletion)
{
    var chatCompletionService = kernel.GetService<IChatCompletion>();

    var chat = chatCompletionService.CreateNewChat("You are an AI assistant that helps people find information.");
    chat.AddMessage(AuthorRole.User, "Hi, what information can yo provide for me?");

    string response = await chatCompletionService.GenerateMessageAsync(chat, new ChatRequestSettings());
    Console.WriteLine(response);
}


