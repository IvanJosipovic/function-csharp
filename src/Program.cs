using function_csharp.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

namespace function_csharp;

//Args:
// --tls_certs_dir: A directory containing tls.crt, tls.key, and ca.crt.
// --address: The address at which to listen for requests.
// --creds: The credentials used to authenticate requests.
// --insecure: Serve insecurely, without credentials or encryption.
// --debug -d

public class Program
{
    public static void Main(string[] args)
    {
        string GetAddress()
        {
            var address = "0.0.0.0:9443";

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--address")
                {
                    address = args[i + 1];
                }
            }

            var insecure = GetInsecure();

            return insecure ? "http://" : GetTLSCertDir() != null ? "https://" : "http://" + address;
        }

        string? GetTLSCertDir()
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--tls_certs_dir")
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        bool GetInsecure()
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--insecure")
                {
                    return true;
                }
            }

            return false;
        }

        var builder = WebApplication.CreateSlimBuilder(args);

        builder.WebHost.UseKestrelHttpsConfiguration();

        builder.WebHost.UseUrls(GetAddress());

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ConfigureEndpointDefaults(lo =>
            {
                lo.Protocols = HttpProtocols.Http2;

                var tls = GetTLSCertDir();

                if (GetInsecure() == false && tls != null)
                {
                    lo.UseHttps(new HttpsConnectionAdapterOptions
                    {
                        ServerCertificate = X509Certificate2.CreateFromPemFile(Path.Combine(tls, "tls.crt"), Path.Combine(tls, "tls.key"))
                    });
                }
            });
        });

        builder.Logging.AddFilter("Default", args.Contains("-d") || args.Contains("--debug") ? LogLevel.Debug : LogLevel.Information);
        builder.Logging.AddFilter("Microsoft.AspNetCore", args.Contains("-d") || args.Contains("--debug") ? LogLevel.Debug : LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Critical);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

        builder.Logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = false;
            options.TimestampFormat = "HH:mm:ss ";
            options.JsonWriterOptions = new()
            {
                Indented = true,
            };
        });

        builder.Services.AddGrpc();

        var app = builder.Build();
        app.Logger.LogInformation("Starting Application");

        app.MapGrpcService<RunFunctionService>();

        app.Run();
    }
}
