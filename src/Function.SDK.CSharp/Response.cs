using Apiextensions.Fn.Proto.V1;
using Google.Protobuf.WellKnownTypes;
using k8s;
using System.Text.Json;

namespace Function.SDK.CSharp;

// https://github.com/crossplane/function-sdk-python/blob/main/crossplane/function/response.py
/// <summary>
/// Utilities for working with RunFunctionResponses.
/// </summary>
public static class Response
{
    /// <summary>
    /// The default TTL for which a RunFunctionResponse may be cached.
    /// </summary>
    public static readonly Duration DefaultTTL = Duration.FromTimeSpan(TimeSpan.FromMinutes(1));

    /// <summary>
    /// Create a response to the supplied request.
    /// </summary>
    /// <param name="request">The request to respond to.</param>
    /// <returns>A response to the supplied request.</returns>
    public static RunFunctionResponse To(this RunFunctionRequest request)
    {
        return request.To(DefaultTTL);
    }

    /// <summary>
    /// Create a response to the supplied request.
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
    /// Add a normal result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Normal(this RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Normal,
            Message = message
        });
    }

    /// <summary>
    /// Add a warning result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Warning(this RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Warning,
            Message = message
        });
    }

    /// <summary>
    /// Add a fatal result to the response.
    /// </summary>
    /// <param name="rsp"></param>
    /// <param name="message"></param>
    public static void Fatal(this RunFunctionResponse rsp, string message)
    {
        rsp.Results.Add(new Result
        {
            Severity = Severity.Fatal,
            Message = message
        });
    }

    /// <summary>
    /// Set the output field in a RunFunctionResponse for operation functions.
    /// Operation functions can return arbitrary output data that will be written
    /// to the Operation's status.pipeline field. This function sets that output
    /// on the response.
    /// </summary>
    /// <param name="rsp">The RunFunctionResponse to update.</param>
    /// <param name="output">The output data as a Dictionary or protobuf Struct.</param>
    /// <exception cref="TypeAccessException">Thrown if the output type is not supported.</exception>
    public static void SetOutput(this RunFunctionResponse rsp, object output)
    {
        rsp.Output = output switch
        {
            Dictionary<string, object> dict => Struct.Parser.ParseJson(JsonSerializer.Serialize(dict)),
            Struct s => s,
            _ => throw new TypeAccessException($"Unsupported output type: {output?.GetType()}"),
        };
    }

    /// <summary>
    /// Adds Desired Resource or merges with an exiting one
    /// </summary>
    /// <param name="state"></param>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    public static void AddOrUpdate(this State state, string key, IKubernetesObject obj)
    {
        var kubeObj = Struct.Parser.ParseJson(KubernetesJson.Serialize(obj));

        if (state.Resources.TryGetValue(key, out Resource? value))
        {
            value.Resource_.MergeFrom(kubeObj);
        }
        else
        {
            state.Resources[key] = new()
            {
                Resource_ = kubeObj
            };
        }
    }

    /// <summary>
    /// Add a resource requirement to the response.
    /// This tells Crossplane to fetch the specified resources and include them
    /// in the next call to the function in req.required_resources[name].
    /// </summary>
    /// <param name="rsp">The RunFunctionResponse to update.</param>
    /// <param name="name">The name to use for this requirement.</param>
    /// <param name="apiVersion">The API version of resources to require.</param>
    /// <param name="kind">The kind of resources to require.</param>
    /// <param name="matchName">Match a resource by name (mutually exclusive with matchLabels).</param>
    /// <param name="matchLabels">Match resources by labels (mutually exclusive with matchName).</param>
    /// <param name="namespace">The namespace to search in (optional).</param>
    /// <exception cref="ArgumentException">Thrown if both matchName and matchLabels are provided, or neither.</exception>
    public static void RequireResources(
        this RunFunctionResponse rsp,
        string name,
        string apiVersion,
        string kind,
        string? matchName = null,
        Dictionary<string, string>? matchLabels = null,
        string? @namespace = null)
    {
        if (matchName == null == (matchLabels == null))
        {
            throw new ArgumentException("Exactly one of matchName or matchLabels must be provided");
        }

        var selector = new ResourceSelector
        {
            ApiVersion = apiVersion,
            Kind = kind
        };

        if (matchName != null)
        {
            selector.MatchName = matchName;
        }

        if (matchLabels != null)
        {
            selector.MatchLabels = new MatchLabels();
            foreach (var kvp in matchLabels)
            {
                selector.MatchLabels.Labels.Add(kvp.Key, kvp.Value);
            }
        }

        if (@namespace != null)
        {
            selector.Namespace = @namespace;
        }

        rsp.Requirements ??= new Requirements();
        rsp.Requirements.Resources[name] = selector;
    }
}