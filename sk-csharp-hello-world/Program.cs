using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

var kernelSettings = KernelSettings.LoadSettings();

var kernelConfig = new KernelConfig();
kernelConfig.AddCompletionBackend(kernelSettings);

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(kernelSettings.LogLevel ?? LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder().WithLogger(loggerFactory.CreateLogger<IKernel>()).WithConfiguration(kernelConfig).Build();

// note: using skills from the repo
var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "FunSkill");

var context = new ContextVariables();
context.Set("input", "Time travel to dinosaur age");
context.Set("style", "Wacky");

var result = await kernel.RunAsync(context, skill["Joke"]);

Console.WriteLine(result);
