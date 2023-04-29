// Copyright (c) Microsoft. All rights reserved.

using System.Net;
using KernelHttpServer.Model;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;


// This endpoint exists as a convenience for the UI to check if the function it is dependent
// on is running. You won't need this endpoint in a typical app.
namespace KernelHttpServer;

public class HealthEndpoint
{
    private readonly IKernel _kernel;

    public HealthEndpoint(IKernel kernel)
    {
        this._kernel = kernel;
    }

    [Function("Health")]
    [OpenApiOperation(operationId: "Health", tags: new[] { "Health" }, Description = "Responds with the health of the service")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HealthResponse), Description = "Health of the service")]
    public async Task<HttpResponseData> HealthAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")]
        HttpRequestData req,
        FunctionContext executionContext)
    {
        var code = HttpStatusCode.OK;
        var message = "OK";

        if (this._kernel == null)
        {
            code = HttpStatusCode.InternalServerError;
            message = "Missing kernel";
        }

        var rep = req.CreateResponse(code);
        await rep.WriteAsJsonAsync(new HealthResponse { Message = message }).ConfigureAwait(false);
        return rep;
    }
}
