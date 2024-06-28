using Google.Protobuf.WellKnownTypes;
using GrpcService;

namespace function_csharp;

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
    /// <param name="ttl">How long Crossplane may optionally cache the response.</param>
    /// <returns>A response to the supplied request.</returns>
    public static RunFunctionResponse To(this RunFunctionRequest request, Duration ttl)
    {
        var resp = new RunFunctionResponse()
        {
            Meta = new ResponseMeta()
            {
                Tag = request.Meta?.Tag ?? "",
                Ttl = ttl
            },
            Desired = request.Desired,
            Context = request.Context,
        };

        return resp;
    }

    /// <summary>
    /// Add a normal result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Normal(RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Normal,
            Message = message
        });
    }

    /// <summary>
    /// Add a warning result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Warning(RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Warning,
            Message = message
        });
    }

    /// <summary>
    /// Add a fatal result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Fatal(RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Fatal,
            Message = message
        });
    }
}