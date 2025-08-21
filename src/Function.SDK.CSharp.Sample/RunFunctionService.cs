using static Apiextensions.Fn.Proto.V1.FunctionRunnerService;
using KubernetesCRDModelGen.Models.azure.m.upbound.io;
using KubernetesCRDModelGen.Models.storage.azure.m.upbound.io;
using Grpc.Core;
using Function.SDK.CSharp.SourceGenerator.Models.platform.example.com;
using Apiextensions.Fn.Proto.V1;
using EnumsNET;

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
        var resp = request.To(Response.DefaultTTL);

        _logger.LogInformation("Running Function");
        resp.Normal("Running Function");

        var observedXR = request.GetObservedCompositeResource<V1alpha1XStorageBucket>();
        var @params = observedXR.Spec.Parameters;

        // Create Resource Group
        var desiredGroup = new V1beta1ResourceGroup()
        {
            ApiVersion = $"{V1beta1ResourceGroup.KubeGroup}/{V1beta1ResourceGroup.KubeApiVersion}",
            Kind = V1beta1ResourceGroup.KubeKind,
            Spec = new V1beta1ResourceGroupSpec
            {
                ForProvider = new()
                {
                    Location = @params.Location.AsString(EnumFormat.EnumMemberValue),
                }
            }
        };

        resp.Desired.AddOrUpdate("rg", desiredGroup);

        // Create Storage Account
        var desiredAccount = new V1beta1Account()
        {
            ApiVersion = $"{V1beta1Account.KubeGroup}/{V1beta1Account.KubeApiVersion}",
            Kind = V1beta1Account.KubeKind,
            Metadata = new()
            {
                Name = observedXR.Metadata.Name.Replace("-", ""),
            },
            Spec = new()
            {
                ForProvider = new()
                {
                    AccountTier = "Standard",
                    AccountReplicationType = "LRS",
                    Location = @params.Location.AsString(EnumFormat.EnumMemberValue),
                    InfrastructureEncryptionEnabled = true,
                    BlobProperties = new()
                    {
                        VersioningEnabled = @params.Versioning
                    },
                    ResourceGroupNameSelector = new()
                    {
                        MatchControllerRef = true
                    }
                }
            }
        };

        resp.Desired.AddOrUpdate("account", desiredAccount);

        // Create Container
        var desiredContainer = new V1beta1Container()
        {
            ApiVersion = $"{V1beta1Container.KubeGroup}/{V1beta1Container.KubeApiVersion}",
            Kind = V1beta1Container.KubeKind,
            Spec = new()
            {
                ForProvider = new()
                {
                    ContainerAccessType = @params.Acl.AsString(EnumFormat.EnumMemberValue),
                    StorageAccountNameSelector = new()
                    {
                        MatchControllerRef = true
                    }
                }
            }
        };

        resp.Desired.AddOrUpdate("container", desiredContainer);

        return Task.FromResult(resp);
    }
}
