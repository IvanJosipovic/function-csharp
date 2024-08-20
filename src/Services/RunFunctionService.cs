using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using k8s;
using KubernetesCRDModelGen.Models.applications.azuread.upbound.io;
using KubernetesCRDModelGen.Models.apiextensions.crossplane.io;
using Google.Protobuf;
using k8s.Models;
using GrpcService;
using function_csharp.Models;

namespace function_csharp.Services;

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

        var envConfig = GetEnvironmentConfig(request);

        var envName = envConfig.Data["environmentName"].ToString();

        var compositeResource = GetCompositeResource<V1alpha1Application>(request);

        var name = $"app-terraform-azure-{envName}-{compositeResource.Metadata.Name}";

        var app = new V1beta1Application()
        {
            ApiVersion = V1beta1Application.KubeApiVersion,
            Kind = V1beta1Application.KubeKind,
            Metadata = new()
            {
                Annotations = new Dictionary<string, string>()
                {
                    //{ "crossplane.io/external-name", name },
                },
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
                    DisplayName = name,
                    Owners = [
                        envConfig.Data["terraformServicePrinciple"]["objectId"]!.ToString()
                    ],
                    PreventDuplicateNames = true,
                    RequiredResourceAccess = [
                    ],
                    Web = [
                        new()
                        {
                            RedirectUris = [
                                "http://localhost:7007/api/auth/microsoft/handler/frame",
                                "https://backstage.aks-dev.app.com/api/auth/microsoft/handler/frame"
                            ]
                        }
                    ],
                }
            }
        };

        resp.Desired.Resources[app.Name()] = new Resource()
        {
            Resource_ = Struct.Parser.ParseJson(KubernetesJson.Serialize(app))
        };

        return Task.FromResult(resp);
    }

    private V1alpha1EnvironmentConfig GetEnvironmentConfig(RunFunctionRequest request)
    {
        var formatterSettings = JsonFormatter.Settings.Default.WithFormatDefaultValues(true);
        string json = new JsonFormatter(formatterSettings).Format(request.Context.Fields["apiextensions.crossplane.io/environment"].StructValue);

        return KubernetesJson.Deserialize<V1alpha1EnvironmentConfig>(json);
    }

    private T GetCompositeResource<T>(RunFunctionRequest request)
    {
        var formatterSettings = JsonFormatter.Settings.Default.WithFormatDefaultValues(true);
        string json = new JsonFormatter(formatterSettings).Format(request.Observed.Composite.Resource_);

        return KubernetesJson.Deserialize<T>(json);
    }
}
