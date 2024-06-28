using function_csharp;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using k8s;
using k8s.Models;

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

        var deployment = new V1Deployment()
        {
            ApiVersion = V1Deployment.KubeApiVersion,
            Kind = V1Deployment.KubeKind,
            Metadata = new()
            {
                Name = "test-deployment",
                NamespaceProperty = "TestNamespace"
            },
            Spec = new()
            {
                Template = new()
                {
                    Spec = new()
                    {
                        Containers =
                        [
                            new()
                            {
                                Name = "test-container",
                                Image = "nginx:latest"
                            }

                        ]
                    }
                }
            }
        };

        resp.Desired.Resources.Add("test", new Resource()
        {
            Resource_ = Struct.Parser.ParseJson(KubernetesJson.Serialize(deployment))
        });

        return Task.FromResult(resp);
    }
}
