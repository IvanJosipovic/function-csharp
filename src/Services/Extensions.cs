using Google.Protobuf.WellKnownTypes;
using k8s;
using Google.Protobuf;
using Apiextensions.Fn.Proto.V1;

namespace Function.SDK.CSharp.Services;

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
