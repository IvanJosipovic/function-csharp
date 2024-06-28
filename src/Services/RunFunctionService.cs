using function_csharp;
using Grpc.Core;

namespace GrpcService.Services;

public class RunFunctionService : FunctionRunnerService.FunctionRunnerServiceBase
{
    private readonly ILogger<RunFunctionService> _logger;

    public RunFunctionService(ILogger<RunFunctionService> logger)
    {
        _logger = logger;
    }

    public override Task<RunFunctionResponse> RunFunction(RunFunctionRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Running Function");

        var resp = request.To(Response.DefaultTTL);

        Response.Normal(resp, "I was here!");

        return Task.FromResult(resp);
    }
}
