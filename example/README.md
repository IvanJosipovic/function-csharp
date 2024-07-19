# Example manifests

You can run your function locally and test it using `crossplane beta render`
with these example manifests.

```shell
# Run the function locally
$ docker build -t function-csharp -f ../src/Dockerfile ../src
$ docker run -it -p 9443:9443 function-csharp
```

```shell
# Then, in another terminal, call it with these example manifests
$ crank beta render xr.yaml composition.yaml functions.yaml --context-files=apiextensions.crossplane.io/environment=./environment-config.json
---
apiVersion: example.crossplane.io/v1
kind: XR
metadata:
  name: example-xr
status:
  conditions:
  - lastTransitionTime: "2024-01-01T00:00:00Z"
    message: 'Unready resources: test'
    reason: Creating
    status: "False"
    type: Ready
---
apiVersion: v1
kind: Deployment
metadata:
  annotations:
    crossplane.io/composition-resource-name: test
  generateName: example-xr-
  labels:
    crossplane.io/composite: example-xr
  name: test-deployment
  namespace: TestNamespace
  ownerReferences:
  - apiVersion: example.crossplane.io/v1
    blockOwnerDeletion: true
    controller: true
    kind: XR
    name: example-xr
    uid: ""
spec:
  template:
    spec:
      containers:
      - image: nginx:latest
        name: test-container
---
apiVersion: render.crossplane.io/v1beta1
kind: Result
message: I was here!
severity: SEVERITY_NORMAL
step: run-the-template
```