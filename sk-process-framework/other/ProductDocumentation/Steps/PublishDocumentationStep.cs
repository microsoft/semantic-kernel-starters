using Microsoft.SemanticKernel;

namespace Steps;

public sealed class PublishDocumentationStep : KernelProcessStep
{
    [KernelFunction]
    public string PublishDocumentation()
    {
        return "Publishing product documentation...";
    }
}
