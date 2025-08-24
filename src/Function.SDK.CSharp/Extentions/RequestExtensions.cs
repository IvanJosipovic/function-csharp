using Apiextensions.Fn.Proto.V1;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using k8s;
using System.Runtime.CompilerServices;

namespace Function.SDK.CSharp;

/// <summary>
/// Package response contains utilities for working with RunFunctionRequests.
/// </summary>
public static partial class RequestExtensions
{
    /// <summary>
    /// The default TTL for which a RunFunctionResponse may be cached.
    /// </summary>
    public static readonly Duration DefaultTTL = Duration.FromTimeSpan(TimeSpan.FromMinutes(1));

    /// <summary>
    /// To bootstraps a response to the supplied request. It automatically copies the desired state from the request.
    /// </summary>
    /// <param name="request">The request to respond to.</param>
    /// <returns>A response to the supplied request.</returns>
    public static RunFunctionResponse To(this RunFunctionRequest request)
    {
        return request.To(DefaultTTL);
    }

    /// <summary>
    /// To bootstraps a response to the supplied request. It automatically copies the desired state from the request.
    /// </summary>
    /// <param name="request">The request to respond to.</param>
    /// <param name="ttl">How long Crossplane may optionally cache the response.</param>
    /// <returns>A response to the supplied request.</returns>
    public static RunFunctionResponse To(this RunFunctionRequest request, Duration ttl)
    {
        var resp = new RunFunctionResponse()
        {
            Meta = new ResponseMeta()
            {
                Tag = request.Meta?.Tag ?? "",
                Ttl = ttl
            },
            Desired = request.Desired,
            Context = request.Context,
        };

        return resp;
    }

    /// <summary>
    /// Get Observed Composite Resource from the supplied request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public static T GetObservedCompositeResource<T>(this RunFunctionRequest request)
    {
        string json = JsonFormatter.Default.Format(request.Observed.Composite.Resource_);

        return KubernetesJson.Deserialize<T>(json);
    }

    /// <summary>
    /// Get Observed Composed Resources from the supplied request.
    /// </summary>
    /// <param name="request">The RunFunctionRequest.</param>
    /// <returns>A dictionary mapping resource names to ObservedComposed objects.</returns>
    /// <exception cref="Exception">Throws if conversion using Resource.AsObject fails.</exception>
    public static IDictionary<string, Resource> GetObservedComposedResources(this RunFunctionRequest request)
    {
        return request.Observed.Resources.ToDictionary();
    }

    /// <summary>
    /// Get Desired Composed Resources from the supplied request.
    /// </summary>
    /// <param name="request">The RunFunctionRequest.</param>
    /// <returns>A dictionary mapping resource names to ObservedComposed objects.</returns>
    /// <exception cref="Exception">Throws if conversion using Resource.AsObject fails.</exception>
    public static IDictionary<string, Resource> GetDesiredComposedResources(this RunFunctionRequest request)
    {
        return request.Desired.Resources.ToDictionary();
    }

    public static IDictionary<string, Resource> UpdateReadyDesiredResources(this RunFunctionRequest request, ILogger _logger)
    {
        var observed = request.GetObservedComposedResources();

        var desired = request.GetDesiredComposedResources();

        foreach (var dr in desired)
        {
            // If this desired resource doesn't exist in the observed resources, it
            // can't be ready because it doesn't yet exist
            if (observed.TryGetValue(dr.Key, out Resource? or))
            {
                // Check if Ready
                if (dr.Value.Ready == Ready.True)
                {
                    _logger.LogDebug("Ignoring desired resource that already has explicit readiness: {name} {ready}", dr.Key, dr.Value.Ready);
                    continue;
                }

                // Now we know this resource exists, and not ready
                _logger.LogDebug("Found desired resource with unknown readiness: {name}", dr.Key);

                // If this observed resource has a status condition with type: Ready,
                // status: True, we set its readiness to true.
                var condition = or.GetCondition("Ready");

                if (condition?.Fields["status"].StringValue == "True")
                {
                    _logger.LogInformation("Automatically determined that composed resource is ready: {name}", dr.Key);
                    dr.Value.Ready = Ready.True;
                }
                else
                {
                    _logger.LogInformation("Automatically determined that composed resource is not ready: {name}", dr.Key);
                    dr.Value.Ready = Ready.False;
                }
            }
            else
            {
                _logger.LogDebug("Ignoring desired resource that does not appear in observed resources: {name}", dr.Key);
                continue;
            }
        }

        return desired;
    }
}
