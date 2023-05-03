// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Models;

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
        HttpRequestData requestData,
        FunctionContext executionContext, string skillName, string functionName)
    {
#pragma warning disable CA1062
        var functionRequest = await JsonSerializer.DeserializeAsync<ExecuteFunctionRequest>(requestData.Body, s_jsonOptions).ConfigureAwait(false);
#pragma warning disable CA1062
        if (functionRequest == null)
        {
            return await CreateResponseAsync(requestData, HttpStatusCode.BadRequest, new ErrorResponse() { Message = $"Invalid request body {functionRequest}" }).ConfigureAwait(false);
        }

        // note: using skills from the repo
        var skillsDirectory = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "skills");
        var skill = this._kernel.ImportSemanticSkillFromDirectory(skillsDirectory, skillName);

        var function = skill[functionName];
        if (function == null)
        {
            return await CreateResponseAsync(requestData, HttpStatusCode.NotFound, new ErrorResponse() { Message = $"Unable to load {skillName}.{functionName}" }).ConfigureAwait(false);
        }

        var context = new ContextVariables();
        foreach (var v in functionRequest.Variables)
        {
            context.Set(v.Key, v.Value);
        }

        var result = await this._kernel.RunAsync(context, function).ConfigureAwait(false);

        return await CreateResponseAsync(requestData, HttpStatusCode.OK, new ExecuteFunctionResponse() { Response = result.ToString() }).ConfigureAwait(false);
    }

    private static async Task<HttpResponseData> CreateResponseAsync(HttpRequestData requestData, HttpStatusCode statusCode, object responseBody)
    {
        var responseData = requestData.CreateResponse(statusCode);
        await responseData.WriteAsJsonAsync(responseBody).ConfigureAwait(false);
        return responseData;
    }
}
