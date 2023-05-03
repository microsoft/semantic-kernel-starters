using Microsoft.SemanticKernel;

internal static class KernelConfigExtensions
{
    /// <summary>
    /// Adds a text completion service to the list. It can be either an OpenAI or Azure OpenAI backend service.
    /// </summary>
    /// <param name="kernelConfig"></param>
    /// <param name="kernelSettings"></param>
    /// <exception cref="ArgumentException"></exception>
    internal static void AddCompletionBackend(this KernelConfig kernelConfig, KernelSettings kernelSettings)
    {
        switch (kernelSettings.ServiceType.ToUpperInvariant())
        {
            case KernelSettings.AzureOpenAI:
                kernelConfig.AddAzureTextCompletionService(kernelSettings.ServiceId, kernelSettings.DeploymentOrModelId, kernelSettings.Endpoint, kernelSettings.ApiKey);
                break;

            case KernelSettings.OpenAI:
                kernelConfig.AddOpenAITextCompletionService(kernelSettings.ServiceId, kernelSettings.DeploymentOrModelId, kernelSettings.ApiKey, kernelSettings.OrgId);
                break;

            default:
                throw new ArgumentException($"Invalid service type value: {kernelSettings.ServiceType}");
        }
    }
}
