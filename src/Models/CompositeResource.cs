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

public partial class V1alpha1ApplicationSpecRequiredResourceAccessResourceAccess
{
    /// <summary>The unique identifier for an app role or OAuth2 permission scope published by the resource application.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Specifies whether the id property references an app role or an OAuth2 permission scope. Possible values are Role or Scope.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public partial class V1alpha1ApplicationSpecRequiredResourceAccess
{
    /// <summary>A collection of resource_access blocks as documented below, describing OAuth2.0 permission scopes and app roles that the application requires from the specified resource.</summary>
    [JsonPropertyName("resourceAccess")]
    public IList<V1alpha1ApplicationSpecRequiredResourceAccessResourceAccess>? ResourceAccess { get; set; }

    /// <summary>The unique identifier for the resource that the application requires access to. This should be the Application ID of the target application.</summary>
    [JsonPropertyName("resourceAppId")]
    public string? ResourceAppId { get; set; }
}

public partial class V1alpha1ApplicationSpecWebImplicitGrant
{
    /// <summary>Whether this web application can request an access token using OAuth 2.0 implicit flow. Whether this web application can request an access token using OAuth 2.0 implicit flow</summary>
    [JsonPropertyName("accessTokenIssuanceEnabled")]
    public bool? AccessTokenIssuanceEnabled { get; set; }

    /// <summary>Whether this web application can request an ID token using OAuth 2.0 implicit flow. Whether this web application can request an ID token using OAuth 2.0 implicit flow</summary>
    [JsonPropertyName("idTokenIssuanceEnabled")]
    public bool? IdTokenIssuanceEnabled { get; set; }
}

public partial class V1alpha1ApplicationSpecWeb
{
    /// <summary>Home page or landing page of the application. Home page or landing page of the application</summary>
    [JsonPropertyName("homepageUrl")]
    public string? HomepageUrl { get; set; }

    /// <summary>An implicit_grant block as documented above.</summary>
    [JsonPropertyName("implicitGrant")]
    public IList<V1alpha1ApplicationSpecWebImplicitGrant>? ImplicitGrant { get; set; }

    /// <summary>The URL that will be used by Microsoft's authorization service to sign out a user using front-channel, back-channel or SAML logout protocols. The URL that will be used by Microsoft's authorization service to sign out a user using front-channel, back-channel or SAML logout protocols</summary>
    [JsonPropertyName("logoutUrl")]
    public string? LogoutUrl { get; set; }

    /// <summary>A set of URLs where user tokens are sent for sign-in, or the redirect URIs where OAuth 2.0 authorization codes and access tokens are sent. Must be a valid http URL or a URN. The URLs where user tokens are sent for sign-in, or the redirect URIs where OAuth 2.0 authorization codes and access tokens are sent</summary>
    [JsonPropertyName("redirectUris")]
    public IList<string>? RedirectUris { get; set; }
}

public partial class V1alpha1ApplicationSpecSinglePageApplication
{
    /// <summary>A set of URLs where user tokens are sent for sign-in, or the redirect URIs where OAuth 2.0 authorization codes and access tokens are sent. Must be a valid https URL. The URLs where user tokens are sent for sign-in, or the redirect URIs where OAuth 2.0 authorization codes and access tokens are sent</summary>
    [JsonPropertyName("redirectUris")]
    public IList<string>? RedirectUris { get; set; }
}

public partial class V1alpha1ApplicationSpec : V1CompositeResourceSpecBase
{
    /// <summary> A collection of user IDs that are allowed to manage this resource.</summary>
    [JsonPropertyName("owners")]
    public IList<string> Owners { get; set; }

    /// <summary>A collection of required_resource_access blocks as documented below.</summary>
    [JsonPropertyName("requiredResourceAccess")]
    public IList<V1alpha1ApplicationSpecRequiredResourceAccess> RequiredResourceAccess { get; set; }

    /// <summary>A single_page_application block as documented below, which configures single-page application (SPA) related settings for this application.</summary>
    [JsonPropertyName("singlePageApplication")]
    public IList<V1alpha1ApplicationSpecSinglePageApplication>? SinglePageApplication { get; set; }

    /// <summary>A web block as documented below, which configures web related settings for this application.</summary>
    [JsonPropertyName("web")]
    public IList<V1alpha1ApplicationSpecWeb>? Web { get; set; }
}