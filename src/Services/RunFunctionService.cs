using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using k8s;
using KubernetesCRDModelGen.Models.applications.azuread.upbound.io;
using KubernetesCRDModelGen.Models.apiextensions.crossplane.io;
using Google.Protobuf;
using k8s.Models;
using Function.SDK.CSharp.Models;
using Apiextensions.Fn.Proto.V1;
using Riok.Mapperly.Abstractions;
using static Apiextensions.Fn.Proto.V1.FunctionRunnerService;

namespace Function.SDK.CSharp.Services;

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

        Response.Normal(resp, "Running Function");

        var compositeResource = request.GetCompositeResource<V1alpha1Application>();

        var name = $"app-terraform-azure-{compositeResource.Metadata.Name}";

        var app = new V1beta1Application()
        {
            ApiVersion = V1beta1Application.KubeGroup + "/" + V1beta1Application.KubeApiVersion,
            Kind = V1beta1Application.KubeKind,
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
                ManagementPolicies =
                [
                    "Observe",
                    "Create",
                    "Update",
                    "LateInitialize",
                ],
                ForProvider = new()
                {
                    DisplayName = name,
                    Owners =
                    [
                        ..compositeResource.Spec.Owners
                    ],
                    PreventDuplicateNames = true,
                    RequiredResourceAccess = compositeResource.Spec.RequiredResourceAccess?.Select(x => new V1beta1ApplicationSpecForProviderRequiredResourceAccess()
                    {
                        ResourceAppId = x.ResourceAppId,
                        ResourceAccess = x.ResourceAccess?.Select(y => new V1beta1ApplicationSpecForProviderRequiredResourceAccessResourceAccess()
                        {
                            Id = y.Id,
                            Type = y.Type
                        }).ToList(),
                    }).ToList(),
                    SinglePageApplication = compositeResource.Spec.SinglePageApplication?.Select(x => new V1beta1ApplicationSpecForProviderSinglePageApplication()
                    {
                        RedirectUris = x.RedirectUris,
                    }).ToList(),
                    Web = compositeResource.Spec.Web?.Select(x => new V1beta1ApplicationSpecForProviderWeb()
                    {
                        HomepageUrl = x.HomepageUrl,
                        ImplicitGrant = x.ImplicitGrant?.Select(y => new V1beta1ApplicationSpecForProviderWebImplicitGrant()
                        {
                            AccessTokenIssuanceEnabled = y.AccessTokenIssuanceEnabled,
                            IdTokenIssuanceEnabled = y.IdTokenIssuanceEnabled
                        }).ToList(),
                        RedirectUris = x.RedirectUris,
                        LogoutUrl = x.LogoutUrl,
                    }).ToList(),
                }
            }
        };

        resp.Desired.AddOrUpdate(name, app);

        return Task.FromResult(resp);
    }
}

[Mapper]
public static partial class StructMapper
{
    public static partial void Update(this Struct existing, Struct update);
}

public static class Extensions
{
    public static T GetCompositeResource<T>(this RunFunctionRequest request)
    {
        var formatterSettings = JsonFormatter.Settings.Default.WithFormatDefaultValues(true);
        string json = new JsonFormatter(formatterSettings).Format(request.Observed.Composite.Resource_);

        return KubernetesJson.Deserialize<T>(json);
    }

    public static void AddOrUpdate(this State state, string key, IKubernetesObject obj)
    {
        var kubeObj = Struct.Parser.ParseJson(KubernetesJson.Serialize(obj));

        if (state.Resources.TryGetValue(key, out Resource? value))
        {
            value.Resource_.Update(kubeObj);
        }
        else
        {
            state.Resources[key] = new()
            {
                Resource_ = kubeObj
            };
        }
    }
}
