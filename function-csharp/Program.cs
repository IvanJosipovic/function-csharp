using GrpcService.Services;
using System.Text.Json;

//Args:
// tls_certs_dir: A directory containing tls.crt, tls.key, and ca.crt.
// function: The function (class) to use to serve requests.
// address: The address at which to listen for requests.
// creds: The credentials used to authenticate requests.
// insecure: Serve insecurely, without credentials or encryption.
// level: What log level to enable.
//   DISABLED = 0
//   DEBUG = 1
//   INFO = 2
var builder = WebApplication.CreateSlimBuilder(args);

builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = false;
    options.TimestampFormat = "HH:mm:ss ";
    options.JsonWriterOptions = new JsonWriterOptions
    {
        Indented = true,
    };
});

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RunFunctionService>();

app.Run();
