using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using static Apiextensions.Fn.Proto.V1.FunctionRunnerService;

namespace Function.SDK.CSharp;

//Args:
// --tls_certs_dir: A directory containing tls.crt, tls.key, and ca.crt.
// --address: The address at which to listen for requests.
// --creds: The credentials used to authenticate requests.
// --insecure: Serve insecurely, without credentials or encryption.
// --debug -d

public static class BuilderExtensions
{
    /// <summary>
    /// Configures the WebApplicationBuilder for Crossplane Function.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <param name="args">The command line arguments passed to the application.</param>
    public static void ConfigureFunction(this WebApplicationBuilder builder, string[] args)
    {
        bool IsDebug()
        {
            return true;
            //return args.Contains("-d") || args.Contains("--debug");
        }

        bool IsInsecure()
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

            var insecure = IsInsecure();

            return (IsInsecure() ? "http://" : GetTLSCertDir() != null ? "https://" : "http://") + address;
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

            return Environment.GetEnvironmentVariable("TLS_SERVER_CERTS_DIR");
        }

        builder.WebHost.UseUrls(GetAddress());

        var tls = GetTLSCertDir();
        if (IsInsecure() == false && tls != null)
        {
            builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate(opts =>
            {
                opts.ChainTrustValidationMode = X509ChainTrustMode.CustomRootTrust;
                opts.CustomTrustStore.Add(X509CertificateLoader.LoadCertificateFromFile(Path.Combine(tls, "ca.crt")));
                opts.AllowedCertificateTypes = CertificateTypes.All;
                opts.RevocationMode = X509RevocationMode.NoCheck;
            });

            builder.WebHost.UseKestrelHttpsConfiguration();
        }
        else
        {
            builder.Services.AddAuthentication();
        }

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.ConfigureEndpointDefaults(lo =>
            {
                lo.Protocols = HttpProtocols.Http2;
            });

            if (IsInsecure() == false && tls != null)
            {
                options.ConfigureHttpsDefaults(options =>
                {
                    options.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                    options.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    options.ServerCertificate = X509Certificate2.CreateFromPemFile(Path.Combine(tls, "tls.crt"), Path.Combine(tls, "tls.key"));
                });
            }
        });

        builder.Logging.AddFilter("Default", IsDebug() ? LogLevel.Debug : LogLevel.Information);
        builder.Logging.AddFilter("Microsoft.AspNetCore", IsDebug() ? LogLevel.Debug : LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Critical);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

        builder.Services.AddGrpc();
        builder.Services.AddGrpcReflection();
    }

    /// <summary>
    /// Configures the WebApplication to the Function gRPC service.
    /// </summary>
    /// <typeparam name="TService">Type which inherits from FunctionRunnerServiceBase</typeparam>
    /// <param name="app"></param>
    public static void MapFunctionService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] TService>(this WebApplication app) where TService : FunctionRunnerServiceBase
    {
        app.Logger.LogInformation("Starting Application");

        app.UseAuthentication();

        app.MapGrpcService<TService>();
        app.MapGrpcReflectionService();
    }
}
