# function-sdk-csharp
The C# SDK for writing [composition functions](https://docs.crossplane.io/latest/composition/compositions/).



## How to Test

You can run your function locally and test it using `crossplane render`
with the example manifests.

### Download Crank and rename to Crossplane
https://releases.crossplane.io/stable/current/bin

## Run Function In IDE
```shell
dotnet debug
```

## Run Function In Docker
```shell
docker build -t function-sdk-csharp -f src/Dockerfile src
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