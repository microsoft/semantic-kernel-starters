using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

string[] scopes = new string[] { "https://cognitiveservices.azure.com/.default" };
var credential = new InteractiveBrowserCredential();
var requestContext = new TokenRequestContext(scopes);
var accessToken = await credential.GetTokenAsync(requestContext);

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Warning)
        .AddConsole()
        .AddDebug();
});

IKernel kernel = new KernelBuilder()
    .WithLogger(loggerFactory.CreateLogger<IKernel>())
    .WithAzureTextCompletionService(
        "text-davinci-003",
        "https://apim...api.net/",
        new BearerTokenCredential(accessToken)
    )
    .Build();

var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
var skill = kernel.ImportSemanticSkillFromDirectory(skillsDirectory, "FunSkill");

var context = new ContextVariables();
context.Set("input", "Time travel to dinosaur age");
context.Set("style", "Wacky");

var result = await kernel.RunAsync(context, skill["Joke"]);

Console.WriteLine(result);
