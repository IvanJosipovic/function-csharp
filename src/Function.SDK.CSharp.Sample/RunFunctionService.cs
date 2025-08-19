using Apiextensions.Fn.Proto.V1;
using Function.SDK.CSharp.SourceGenerator.Models.data.company.com;
using Grpc.Core;
using KubernetesCRDModelGen.Models.security.databricks.crossplane.io;
using static Apiextensions.Fn.Proto.V1.FunctionRunnerService;

namespace Function.SDK.CSharp.Sample;

public class RunFunctionService : FunctionRunnerServiceBase
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

        resp.Normal("Running Function");

        var compositeResource = request.GetCompositeResource<V1alpha1ETL>();

        var name = $"perm-{compositeResource.Metadata.Name}";

        var perm = new V1alpha1Permissions()
        {
            ApiVersion = V1alpha1Permissions.KubeGroup + "/" + V1alpha1Permissions.KubeApiVersion,
            Kind = V1alpha1Permissions.KubeKind,
            Metadata = new()
            {
                Labels = new Dictionary<string, string>()
                {
                    { "app.com/name", name }
                },
                Name = name,
            },
            Spec = new()
            {
                ForProvider = new()
                {
                    AccessControl = [
                        new() {
                            GroupName = "my-grou-1",
                            PermissionLevel = "CAN_MANAGE"
                        }
                    ]
                }
            }
        };

        resp.Desired.AddOrUpdate(name, perm);

        return Task.FromResult(resp);
    }
}
