# function-sdk-csharp
The C# SDK for writing [composition functions](https://docs.crossplane.io/latest/composition/compositions/).

## Features
- XRD to Model Generation
  - Modify the xrd.yaml and models will be automatically generated
- CRD to Model Generation
  - Add crd.yaml(s) to the project and models will be automatically generated
  - Most Crossplane Providers already published [KubernetesCRDModelGen](https://github.com/IvanJosipovic/KubernetesCRDModelGen?tab=readme-ov-file#published-packages)
    | Group | NuGet |
    |---|---|
    | aws.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.aws.upbound.io/) |
    | azapi.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.azapi.upbound.io/) |
    | azure.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.azure.upbound.io/) |
    | azuread.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.azuread.upbound.io/) |
    | crossplane.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.crossplane.io/) |
    | databricks.crossplane.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.databricks.crossplane.io/) |
    | gcp.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.gcp.upbound.io/) |
    | helm.crossplane.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.helm.crossplane.io/) |
    | kubernetes.crossplane.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.kubernetes.crossplane.io/) |
    | opentofu.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.opentofu.upbound.io/) |
    | tf.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.tf.upbound.io/) |
    | upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.upbound.io/) |
    | vault.upbound.io | [Link](https://www.nuget.org/packages/KubernetesCRDModelGen.Models.vault.upbound.io/) |

## How to Test

You can run your function locally and test it using `crossplane render`
with the example manifests.

### Download Crank and rename to Crossplane
https://releases.crossplane.io/stable/current/bin

## Run Function In IDE
Download the lastest [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
```shell
dotnet debug
```

## Run Function In Docker
```shell
docker build -t function-sdk-csharp -f src/Function.SDK.CSharp/Dockerfile src
docker run -it -p 9443:9443 function-sdk-csharp
```

## Run Test
Then, in another terminal, call it with these example manifests
```
crossplane render example/xr.yaml example/composition.yaml example/functions.yaml
```

```yaml
---
apiVersion: data.company.com/v1alpha1
kind: ETL
metadata:
  name: example-xr
status:
  conditions:
  - lastTransitionTime: "2024-01-01T00:00:00Z"
    message: 'Unready resources: perm-example-xr'
    reason: Creating
    status: "False"
    type: Ready
---
apiVersion: security.databricks.crossplane.io/v1alpha1
kind: Permissions
metadata:
  annotations:
    crossplane.io/composition-resource-name: perm-example-xr
  generateName: example-xr-
  labels:
    app.com/name: perm-example-xr
    crossplane.io/composite: example-xr
  name: perm-example-xr
  ownerReferences:
  - apiVersion: data.company.com/v1alpha1
    blockOwnerDeletion: true
    controller: true
    kind: ETL
    name: example-xr
    uid: ""
spec:
  forProvider:
    accessControl:
    - groupName: my-grou-1
      permissionLevel: CAN_MANAGE

```
