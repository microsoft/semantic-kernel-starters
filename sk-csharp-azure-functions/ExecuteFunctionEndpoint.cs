// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System.Text.Json;
using System.Linq;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.KernelExtensions;
using Models;
using Tavis.UriTemplates;

public class ExecuteFunctionEndpoint
{
    private static readonly JsonSerializerOptions s_jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IKernel _kernel;

    public ExecuteFunctionEndpoint(IKernel kernel)
    {
        this._kernel = kernel;
    }

    [Function("ExecuteFunction")]
    [OpenApiOperation(operationId: "ExecuteFunction", tags: new[] { "ExecuteFunction" }, Description = "Execute the specified semantic function")]
    [OpenApiParameter(name: "skillName", Description = "Name of the skill")]
    [OpenApiParameter(name: "functionName", Description = "Name of the function to execute")]
    [OpenApiRequestBody("application/json", typeof(ExecuteFunctionRequest))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ExecuteFunctionResponse), Description = "Includes the AI response")]
    public async Task<HttpResponseData> ExecuteFunctionAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "skills/{skillName}/functions/{functionName}")]
        HttpRequestData req,
        FunctionContext executionContext, string skillName, string functionName)
    {
        var funcReq = await JsonSerializer.DeserializeAsync<ExecuteFunctionRequest>(req.Body, s_jsonOptions).ConfigureAwait(false);

        // note: using skills from the repo
        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
        var skill = _kernel.ImportSemanticSkillFromDirectory(skillsDirectory, skillName);
        var skfunction = skill[functionName];

        var context = new ContextVariables();
        foreach (var v in funcReq.Variables)
        {
            context.Set(v.Key, v.Value);
        }

        var result = await _kernel.RunAsync(context, skfunction);

        var rep = req.CreateResponse(HttpStatusCode.OK);
        await rep.WriteAsJsonAsync(new ExecuteFunctionResponse() { Response = result.ToString() }).ConfigureAwait(false);
        return rep;
    }
}
