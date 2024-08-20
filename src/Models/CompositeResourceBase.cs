using k8s;
using k8s.Models;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace function_csharp.Models;

public enum V1CompositeResourceSpecCompositeDeletePolicyEnum
{
    [EnumMember(Value = "Background")]
    /// <summary>Background</summary>
    Background,
    [EnumMember(Value = "Foreground")]
    /// <summary>Foreground</summary>
    Foreground
}

public partial class V1CompositeResourceSpecCompositionRef
{
    /// <summary></summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public partial class V1CompositeResourceSpecCompositionRevisionRef
{
    /// <summary></summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public partial class V1CompositeResourceSpecCompositionRevisionSelector
{
    /// <summary></summary>
    [JsonPropertyName("matchLabels")]
    public IDictionary<string, string> MatchLabels { get; set; }
}

public partial class V1CompositeResourceSpecCompositionSelector
{
    /// <summary></summary>
    [JsonPropertyName("matchLabels")]
    public IDictionary<string, string> MatchLabels { get; set; }
}

public enum V1CompositeResourceSpecCompositionUpdatePolicyEnum
{
    [EnumMember(Value = "Automatic")]
    /// <summary>Automatic</summary>
    Automatic,
    [EnumMember(Value = "Manual")]
    /// <summary>Manual</summary>
    Manual
}

public partial class V1CompositeResourceSpecPublishConnectionDetailsToConfigRef
{
    /// <summary></summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public partial class V1CompositeResourceSpecPublishConnectionDetailsToMetadata
{
    /// <summary></summary>
    [JsonPropertyName("annotations")]
    public IDictionary<string, string>? Annotations { get; set; }

    /// <summary></summary>
    [JsonPropertyName("labels")]
    public IDictionary<string, string>? Labels { get; set; }

    /// <summary></summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public partial class V1CompositeResourceSpecPublishConnectionDetailsTo
{
    /// <summary></summary>
    [JsonPropertyName("configRef")]
    public V1CompositeResourceSpecPublishConnectionDetailsToConfigRef? ConfigRef { get; set; }

    /// <summary></summary>
    [JsonPropertyName("metadata")]
    public V1CompositeResourceSpecPublishConnectionDetailsToMetadata? Metadata { get; set; }

    /// <summary></summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public partial class V1CompositeResourceSpecResourceRef
{
    /// <summary></summary>
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; }

    /// <summary></summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    /// <summary></summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public partial class V1CompositeResourceSpecWriteConnectionSecretToRef
{
    /// <summary></summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public partial class V1CompositeResourceStatusConditions
{
    /// <summary></summary>
    [JsonPropertyName("lastTransitionTime")]
    public string LastTransitionTime { get; set; }

    /// <summary></summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary></summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; }

    /// <summary></summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary></summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public partial class V1CompositeResourceStatusConnectionDetails
{
    /// <summary></summary>
    [JsonPropertyName("lastPublishedTime")]
    public string? LastPublishedTime { get; set; }
}

public partial class V1CompositeResourceStatus
{
    /// <summary>Conditions of the resource.</summary>
    [JsonPropertyName("conditions")]
    public IList<V1CompositeResourceStatusConditions>? Conditions { get; set; }

    /// <summary></summary>
    [JsonPropertyName("connectionDetails")]
    public V1CompositeResourceStatusConnectionDetails? ConnectionDetails { get; set; }
}

public abstract class V1CompositeResourceSpecBase
{
    /// <summary></summary>
    [JsonPropertyName("compositeDeletePolicy")]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public V1CompositeResourceSpecCompositeDeletePolicyEnum? CompositeDeletePolicy { get; set; }

    /// <summary></summary>
    [JsonPropertyName("compositionRef")]
    public V1CompositeResourceSpecCompositionRef? CompositionRef { get; set; }

    /// <summary></summary>
    [JsonPropertyName("compositionRevisionRef")]
    public V1CompositeResourceSpecCompositionRevisionRef? CompositionRevisionRef { get; set; }

    /// <summary></summary>
    [JsonPropertyName("compositionRevisionSelector")]
    public V1CompositeResourceSpecCompositionRevisionSelector? CompositionRevisionSelector { get; set; }

    /// <summary></summary>
    [JsonPropertyName("compositionSelector")]
    public V1CompositeResourceSpecCompositionSelector? CompositionSelector { get; set; }

    /// <summary></summary>
    [JsonPropertyName("compositionUpdatePolicy")]
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public V1CompositeResourceSpecCompositionUpdatePolicyEnum? CompositionUpdatePolicy { get; set; }

    /// <summary></summary>
    [JsonPropertyName("publishConnectionDetailsTo")]
    public V1CompositeResourceSpecPublishConnectionDetailsTo? PublishConnectionDetailsTo { get; set; }

    /// <summary></summary>
    [JsonPropertyName("resourceRef")]
    public V1CompositeResourceSpecResourceRef? ResourceRef { get; set; }

    /// <summary></summary>
    [JsonPropertyName("writeConnectionSecretToRef")]
    public V1CompositeResourceSpecWriteConnectionSecretToRef? WriteConnectionSecretToRef { get; set; }
}