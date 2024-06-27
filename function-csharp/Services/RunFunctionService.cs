using Google.Protobuf.WellKnownTypes;
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
        _logger.LogInformation(request.Meta.Tag);

        var resp = request.To(Response.DefaultTTL);

        return Task.FromResult(resp);
    }
}

// https://github.com/crossplane/function-sdk-python/blob/main/crossplane/function/response.py
public static class Response
{
    /// <summary>
    /// The default TTL for which a RunFunctionResponse may be cached.
    /// </summary>
    public static readonly Duration DefaultTTL = Duration.FromTimeSpan(TimeSpan.FromMinutes(1));

    /// <summary>
    /// Create a response to the supplied request.
    /// </summary>
    /// <param name="request">The request to respond to.</param>
    /// <param name=""></param>
    /// <returns>A response to the supplied request.</returns>
    public static RunFunctionResponse To(this RunFunctionRequest request, Duration ttl)
    {
        var resp = new RunFunctionResponse()
        {
            Meta = new ResponseMeta()
            {
                Tag = request.Meta.Tag,
                Ttl = ttl
            },
            Desired = request.Desired,
            Context = request.Context,
        };

        return resp;
    }
}