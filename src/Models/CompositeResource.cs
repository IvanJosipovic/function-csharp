using k8s;
using k8s.Models;
using System.Text.Json.Serialization;

namespace Function.SDK.CSharp.Models;

[KubernetesEntity(Group = KubeGroup, Kind = KubeKind, ApiVersion = KubeApiVersion, PluralName = KubePluralName)]
public partial class V1alpha1ETL : IKubernetesObject<V1ObjectMeta>, ISpec<V1alpha1ApplicationSpec>, IStatus<V1CompositeResourceStatus>
{
    public const string KubeApiVersion = "v1alpha1";
    public const string KubeKind = "ETL";
    public const string KubeGroup = "data.company.com";
    public const string KubePluralName = "etls";

    /// <summary></summary>
    [JsonPropertyName("apiVersion")]
    public string ApiVersion { get; set; }

    /// <summary></summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; }

    /// <summary></summary>
    [JsonPropertyName("metadata")]
    public V1ObjectMeta Metadata { get; set; }

    /// <summary></summary>
    [JsonPropertyName("spec")]
    public V1alpha1ApplicationSpec Spec { get; set; }

    /// <summary></summary>
    [JsonPropertyName("status")]
    public V1CompositeResourceStatus Status { get; set; }
}

public partial class V1alpha1ApplicationSpec : V1CompositeResourceSpecBase
{
    /// <summary> A collection of user IDs that are allowed to manage this resource.</summary>
    [JsonPropertyName("owners")]
    public IList<string> Owners { get; set; }

}