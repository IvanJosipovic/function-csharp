using function_csharp;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using k8s;
using k8s.Models;
using KubernetesCRDModelGen.Models.applications.azuread.upbound.io;
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

        Response.Normal(resp, "Running Function");

        var name = $"app-terraform-azure-{request.Context.Fields["apiextensions.crossplane.io/environment"].StructValue.Fields["data"].StructValue.Fields["environmentName"].StringValue}-{request.Observed.Composite.Resource_.Fields["metadata"].StructValue.Fields["name"].StringValue}";

        var app = new V1beta1Application()
        {
            ApiVersion = V1beta1Application.KubeApiVersion,
            Kind = V1beta1Application.KubeKind,
            Metadata = new()
            {
                Annotations = new Dictionary<string, string>()
                {
                    { "crossplane.io/external-name", name },
                },
                Labels = new Dictionary<string, string>()
                {
                    { "da.teck.com/alloy-name", name }
                },
                Name = name,
            },
            Spec = new()
            {
                ForProvider = new()
                {
                    DisplayName = name,
                    Owners = [
                        request.Context.Fields["apiextensions.crossplane.io/environment"].StructValue.Fields["data"].StructValue.Fields["terraformServicePrinciple"].StructValue.Fields["objectId"].StringValue
                    ],
                    RequiredResourceAccess = [
                    ],
                    Web = [
                        new(){
                            RedirectUris = [
                                "http://localhost:7007/api/auth/microsoft/handler/frame",
                                "https://backstage.aks-dev.galileo.teck.com/api/auth/microsoft/handler/frame"
                            ]
                        }
                    ],
                }
            }
        };

        resp.Desired.Resources.Add("application", new Resource()
        {
            Resource_ = Struct.Parser.ParseJson(KubernetesJson.Serialize(app))
        });

        return Task.FromResult(resp);
    }
}
