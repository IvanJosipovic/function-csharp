# Example manifests

You can run your function locally and test it using `crossplane beta render`
with these example manifests.

```shell
# Run the function locally
$ docker build -t function-csharp -f ../src/Dockerfile ../src
$ docker run -it -p [::1]:9443:9443 function-csharp 
```

```shell
# Then, in another terminal, call it with these example manifests
$ crank beta render xr.yaml composition.yaml functions.yaml -r
---
apiVersion: example.crossplane.io/v1
kind: XR
metadata:
  name: example-xr
---
apiVersion: render.crossplane.io/v1beta1
kind: Result
message: I was run with input "Hello world"!
severity: SEVERITY_NORMAL
step: run-the-template
```