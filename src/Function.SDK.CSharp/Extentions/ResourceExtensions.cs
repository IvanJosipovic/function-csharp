using Apiextensions.Fn.Proto.V1;
using Google.Protobuf.WellKnownTypes;

namespace Function.SDK.CSharp;

public static class ResourceExtensions
{
    /// <summary>
    /// Gets the Resource condition by name
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="condition"></param>
    /// <returns></returns>
    public static Struct? GetCondition(this Resource resource, string condition)
    {
        var conditions = resource.Resource_.Fields["status"].StructValue.Fields["conditions"].ListValue.Values;

        foreach (var value in conditions)
        {
            if (value.StructValue.Fields["type"].StringValue == condition)
            {
                return value.StructValue;
            }
        }

        return null;
    }
}