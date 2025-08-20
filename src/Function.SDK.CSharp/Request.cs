using Google.Protobuf.WellKnownTypes;
using k8s;
using Google.Protobuf;
using Apiextensions.Fn.Proto.V1;

namespace Function.SDK.CSharp;

public static class Request
{
    public static T GetObservedCompositeResource<T>(this RunFunctionRequest request)
    {
        string json = JsonFormatter.Default.Format(request.Observed.Composite.Resource_);

        return KubernetesJson.Deserialize<T>(json);
    }
}
