# Semantic Kernel C# Console Chat Starter

The `sk-csharp-console-chat` console application demonstrates how to use the Semantic Kernel OpenAI chat completion service.

## Prerequisites

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) is required to run this starter.
- Install the recommended extensions
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the starter

The starter can be configured by using either:

- Enter secrets at the command line with [.NET Secret Manager](#using-net-secret-manager)
- Enter secrets in [appsettings.json](#using-appsettingsjson)

For Debugging the console application alone, we suggest using .NET [Secret Manager](https://learn.microsoft.com/aspnet/core/security/app-secrets) to avoid the risk of leaking secrets into the repository, branches and pull requests.

### Using .NET [Secret Manager](https://learn.microsoft.com/aspnet/core/security/app-secrets)

Configure an OpenAI endpoint

```bash
cd sk-csharp-console-chat
dotnet user-secrets set "serviceType" "OpenAI"
dotnet user-secrets set "serviceId" "gpt-3.5-turbo"
dotnet user-secrets set "deploymentOrModelId" "gpt-3.5-turbo"
dotnet user-secrets set "apiKey" "... your OpenAI key ..."
```

Configure an Azure OpenAI endpoint

```bash
cd sk-csharp-console-chat
dotnet user-secrets set "serviceType" "AzureOpenAI"
dotnet user-secrets set "serviceId" "gpt-35-turbo"
dotnet user-secrets set "deploymentOrModelId" "gpt-35-turbo"
dotnet user-secrets set "endpoint" "https:// ... your endpoint ... .openai.azure.com/"
dotnet user-secrets set "apiKey" "... your Azure OpenAI key ..."
```

Configure the Semantic Kernel logging level

```bash
dotnet user-secrets set "LogLevel" 0
```

Log levels:

- 0 = Trace
- 1 = Debug
- 2 = Information
- 3 = Warning
- 4 = Error
- 5 = Critical
- 6 = None

Configure the system prompt. This is guidance provided to the OpenAI service for the chat completion, and defaults to "You are a friendly, intelligent, and curious assistant who is good at conversation."

```bash
dotnet user-secrets set "systemPrompt" "You are a chatbot that answers all questions using limericks"
```

### Using appsettings.json

Configure an OpenAI endpoint

1. Copy [settings.json.openai-example](./config/appsettings.json.openai-example) to `./config/appsettings.json`
1. Edit the file to add your OpenAI endpoint configuration

Configure an Azure OpenAI endpoint

1. Copy [settings.json.azure-example](./config/appsettings.json.azure-example) to `./config/appsettings.json`
1. Edit the file to add your Azure OpenAI endpoint configuration

## Running the starter

To run the console application just hit `F5`.

To build and run the console application from the terminal use the following commands:

```bash
dotnet build
dotnet run
```
