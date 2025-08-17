using Apiextensions.Fn.Proto.V1;
using Function.SDK.CSharp.Models;
using Function.SDK.CSharp.Services;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Testing;
using Grpc.Core.Utils;
using k8s;
using KubernetesCRDModelGen.Models.security.databricks.crossplane.io;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace Function.SDK.CSharp.Test;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var xr = new V1alpha1ETL()
        {
            Metadata = new()
            {
                Name = "test",
                NamespaceProperty = "default"
            },
            Spec = new()
            {
                Owners = [
                            "test1",
                            "test2"
                         ]
            }
        };

        var request = TestExtensions.GetFunctionRequest();
        request.SetCompositeResource(xr);

        var response1 = request.GetTestResponse();

        var desiredResource = new V1alpha1Permissions()
        {
            ApiVersion = V1alpha1Permissions.KubeGroup + "/" + V1alpha1Permissions.KubeApiVersion,
            Kind = V1alpha1Permissions.KubeKind,
            Metadata = new()
            {
                Labels = new Dictionary<string, string>()
                {
                    { "app.com/name", "perm-test" }
                },
                Name = "perm-test",
            },
            Spec = new()
            {
                ForProvider = new()
                {
                    AccessControl = [
                        new()
                        {
                            GroupName = "my-grou-1",
                            PermissionLevel = "CAN_MANAGE"
                        }
                    ]
                }
            }
        };

        response1.GetDesiredResource<V1alpha1Permissions>("perm-test").ShouldBeEquivalentTo(desiredResource);

        // Update Desired Resource Status and rerun function

        var perm2 = new V1alpha1Permissions()
        {
            ApiVersion = V1alpha1Permissions.KubeGroup + "/" + V1alpha1Permissions.KubeApiVersion,
            Kind = V1alpha1Permissions.KubeKind,
            Metadata = new()
            {
                Labels = new Dictionary<string, string>()
                {
                    { "app.com/name", "perm-test" }
                },
                Name = "perm-test",
            },
            Spec = new()
            {
                ForProvider = new()
                {
                    AccessControl = [
                new()
                        {
                            GroupName = "my-grou-1",
                            PermissionLevel = "CAN_MANAGE"
                        }
            ]
                }
            },
            Status = new()
            {
                ObservedGeneration = 1
            }
        };

        var request2 = TestExtensions.GetFunctionRequest();
        request2.SetCompositeResource(xr);

        request2.Desired.AddOrUpdate("perm-test", perm2);

        var response2 = request2.GetTestResponse();

        response2.Desired.GetResource<V1alpha1Permissions>("perm-test").ShouldBeEquivalentTo(perm2);
    }
}

public static class TestExtensions
{
    public static RunFunctionRequest GetFunctionRequest()
    {
        var request = new RunFunctionRequest
        {
            Observed = new()
            {
                Composite = new()
            },
            Desired = new(),
            Context = new(),
        };

        return request;
    }

    public static RunFunctionResponse GetTestResponse(this RunFunctionRequest request)
    {
        var svc = new RunFunctionService(new LoggerFactory().CreateLogger<RunFunctionService>());
        var fakeServerCallContext = TestServerCallContext.Create("/apiextensions.fn.proto.v1.FunctionRunnerService/RunFunction", null, DateTime.UtcNow.AddHours(1), [], CancellationToken.None, "127.0.0.1", null, null, (metadata) => TaskUtils.CompletedTask, () => new WriteOptions(), (writeOptions) => { });

        return svc.RunFunction(request, fakeServerCallContext)
            .GetAwaiter()
            .GetResult();
    }

    public static void SetCompositeResource(this RunFunctionRequest request, IKubernetesObject obj)
    {
        var kubeObj = Struct.Parser.ParseJson(KubernetesJson.Serialize(obj));
        request.Observed.Composite.Resource_ = kubeObj;
    }

    public static T GetResource<T>(this State state, string key)
    {
        string json = JsonFormatter.Default.Format(state.Resources[key].Resource_);

        return KubernetesJson.Deserialize<T>(json);
    }
}