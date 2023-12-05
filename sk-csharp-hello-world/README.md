# Semantic Kernel C# Hello World Starter

The `sk-csharp-hello-world` console application demonstrates how to execute a semantic function.

## Prerequisites

- [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) is required to run this starter.
- Install the recommended extensions
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the starter

The starter can be configured by using either:

- Enter secrets at the command line with [.NET Secret Manager](#using-net-secret-manager)
- Enter secrets in [appsettings.json](#using-appsettingsjson)

For Debugging the console application alone, we suggest using .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to avoid the risk of leaking secrets into the repository, branches and pull requests.

### Using .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)

Configure an OpenAI endpoint

```powershell
cd sk-csharp-hello-world
dotnet user-secrets set "serviceType" "OpenAI"
dotnet user-secrets set "serviceId" "gpt-3.5-turbo"
dotnet user-secrets set "modelId" "gpt-3.5-turbo"
dotnet user-secrets set "apiKey" "... your OpenAI key ..."
```

Configure an Azure OpenAI endpoint

```powershell
cd sk-csharp-hello-world
dotnet user-secrets set "serviceType" "AzureOpenAI"
dotnet user-secrets set "serviceId" "gpt-35-turbo"
dotnet user-secrets set "deploymentId" "gpt-35-turbo"
dotnet user-secrets set "modelId" "gpt-3.5-turbo"
dotnet user-secrets set "endpoint" "https:// ... your endpoint ... .openai.azure.com/"
dotnet user-secrets set "apiKey" "... your Azure OpenAI key ..."
```

Configure the Semantic Kernel logging level

```powershell
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

### Using appsettings.json

Configure an OpenAI endpoint

1. Copy [settings.json.openai-example](./config/appsettings.json.openai-example) to `./Config/appsettings.json`
1. Edit the file to add your OpenAI endpoint configuration

Configure an Azure OpenAI endpoint

1. Copy [settings.json.azure-example](./config/appsettings.json.azure-example) to `./Config/appsettings.json`
1. Edit the file to add your Azure OpenAI endpoint configuration

## Running the starter

To run the console application just hit `F5`.

To build and run the console application from the terminal use the following commands:

```powershell
dotnet build
dotnet run
```

## Troubleshooting

### Getting a 400 (BadRequest) and error "Azure.RequestFailedException: logprobs, best_of and echo parameters are not available on gpt-35-turbo model. Please remove the parameter and try again."

A chat completion model (gpt-35-turbo) was set in serviceId/deploymentOrModelId while the kernel was configured to use a text completion model. The type of model used by the kernel can be configured with the endpointType secret. To fix, you can either:

- change endpointType to chat-completion

```powershell
dotnet user-secrets set "endpointType" "chat-completion"
```

- change serviceId and deploymentOrModelId to a text completion service like "text-davinci-003": [See Using .NET Secret Manager](#using-net-secret-manager). Please note that the [text-davinci-003 model will be phased out in the future](https://techcommunity.microsoft.com/t5/azure-ai-services-blog/announcing-updates-to-azure-openai-service-models/ba-p/3866757).
