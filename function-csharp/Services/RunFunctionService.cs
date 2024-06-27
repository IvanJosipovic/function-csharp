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
        return base.RunFunction(request, context);
    }
}
