using Microsoft.SemanticKernel;

internal static class KernelBuilderExtensions
{
    /// <summary>
    /// Adds a chat completion service to the list. It can be either an OpenAI or Azure OpenAI backend service.
    /// </summary>
    /// <param name="kernelBuilder"></param>
    /// <param name="kernelSettings"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static KernelBuilder WithChatCompletionService(this KernelBuilder kernelBuilder, KernelSettings kernelSettings)
    {
        switch (kernelSettings.ServiceType.ToUpperInvariant())
        {
            case ServiceTypes.AzureOpenAI:
                kernelBuilder = kernelBuilder.WithAzureOpenAIChatCompletion(kernelSettings.DeploymentId, kernelSettings.ModelId, endpoint: kernelSettings.Endpoint, apiKey: kernelSettings.ApiKey, serviceId: kernelSettings.ServiceId);
                break;

            case ServiceTypes.OpenAI:
                kernelBuilder = kernelBuilder.WithOpenAIChatCompletion(modelId: kernelSettings.ModelId, apiKey: kernelSettings.ApiKey, orgId: kernelSettings.OrgId, serviceId: kernelSettings.ServiceId);
                break;

            default:
                throw new ArgumentException($"Invalid service type value: {kernelSettings.ServiceType}");
        }

        return kernelBuilder;
    }
}
