# Example manifests

You can run your function locally and test it using `crossplane render`
with these example manifests.

### Download Crank and rename to Crossplane
https://releases.crossplane.io/stable/current/bin


# Run Function In Docker
```shell
# modify functions.yaml and set render.crossplane.io/runtime to Docker

$ docker build -t function-csharp -f ../src/Dockerfile ../src
$ docker run -it -p 9443:9443 function-csharp
```

# Run Function In IDE
```shell
dotnet debug
```

# Run Test
```shell
# Then, in another terminal, call it with these example manifests
$ crossplane render xr.yaml composition.yaml functions.yaml
---
apiVersion: azuread.company.com/v1alpha1
kind: xApplication
metadata:
  name: example-xr
status:
  conditions:
  - lastTransitionTime: "2024-01-01T00:00:00Z"
    message: 'Unready resources: app-terraform-azure-da-platform-dev-example-xr'
    reason: Creating
    status: "False"
    type: Ready
---
apiVersion: applications.azuread.upbound.io/v1beta1
kind: Application
metadata:
  annotations:
    crossplane.io/composition-resource-name: app-terraform-azure-da-platform-dev-example-xr
  generateName: example-xr-
  labels:
    app.com/name: app-terraform-azure-da-platform-dev-example-xr
    crossplane.io/composite: example-xr
  name: app-terraform-azure-da-platform-dev-example-xr
  ownerReferences:
  - apiVersion: azuread.company.com/v1alpha1
    blockOwnerDeletion: true
    controller: true
    kind: xApplication
    name: example-xr
    uid: ""
spec:
  forProvider:
    displayName: app-terraform-azure-da-platform-dev-example-xr
    owners:
    - 3983c9fb-06c6-4306-b917-d9b68fe45a50
    - owner1@example.com
    - owner2@example.com
    preventDuplicateNames: true
    requiredResourceAccess:
    - resourceAccess:
      - id: 11111111-1111-1111-1111-111111111111
        type: Scope
      - id: 22222222-2222-2222-2222-222222222222
        type: Role
      resourceAppId: 12345678-1234-1234-1234-1234567890ab
    singlePageApplication:
    - redirectUris:
      - https://example.com/callback
      - https://example.com/redirect
    web:
    - homepageUrl: https://example.com
      implicitGrant:
      - accessTokenIssuanceEnabled: true
        idTokenIssuanceEnabled: false
      logoutUrl: https://example.com/logout
      redirectUris:
      - https://example.com/callback1
      - https://example.com/callback2
  managementPolicies:
  - Observe
  - Create
  - Update
  - LateInitialize
```