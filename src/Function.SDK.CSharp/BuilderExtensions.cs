using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Diagnostics.CodeAnalysis;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using static Apiextensions.Fn.Proto.V1.FunctionRunnerService;
using static System.Net.WebRequestMethods;

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

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;

            options.ConfigureEndpointDefaults(listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;

                if (IsInsecure() == false)
                {
                    var tls = GetTLSCertDir();
                    if (tls != null)
                    {
                        var ca = X509CertificateLoader.LoadCertificateFromFile(Path.Combine(tls, "ca.crt"));
                        var serverCert = X509Certificate2.CreateFromPemFile(Path.Combine(tls, "tls.crt"), Path.Combine(tls, "tls.key"));

                        var adapter = new HttpsConnectionAdapterOptions
                        {
                            CheckCertificateRevocation = false,
                            ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                            ServerCertificate = serverCert,
                            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                            ServerCertificateChain = [.. new X509Certificate2[] { ca, serverCert }],

                            ClientCertificateValidation = (cert, chain, errors) =>
                            {
                                using var custom = new X509Chain
                                {
                                    ChainPolicy =
                                    {
                                        TrustMode = X509ChainTrustMode.CustomRootTrust,
                                        RevocationMode = X509RevocationMode.NoCheck,
                                        VerificationFlags = X509VerificationFlags.NoFlag,
                                        DisableCertificateDownloads = true
                                    }
                                };

                                custom.ChainPolicy.CustomTrustStore.Add(ca);

                                return custom.Build(cert);
                            },
                        };

                        // Disables Client Cert Validation
                        //adapter.AllowAnyClientCertificate();

                        listenOptions.UseHttps(adapter);
                    }
                }
            });
        });

        builder.Logging.AddFilter("Default", IsDebug() ? LogLevel.Debug : LogLevel.Information);
        builder.Logging.AddFilter("Microsoft.AspNetCore", IsDebug() ? LogLevel.Debug : LogLevel.Warning);
        builder.Logging.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Critical);
        builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Warning);

        //builder.Logging.AddJsonConsole(options =>
        //{
        //    options.IncludeScopes = false;
        //    options.TimestampFormat = "HH:mm:ss ";
        //    options.JsonWriterOptions = new()
        //    {
        //        Indented = true,
        //    };
        //});

        builder.Services.AddGrpc();
    }

    /// <summary>
    /// Configures the WebApplication to the Function gRPC service.
    /// </summary>
    /// <typeparam name="TService">Type which inherits from FunctionRunnerServiceBase</typeparam>
    /// <param name="app"></param>
    public static void MapFunctionService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] TService>(this WebApplication app) where TService : FunctionRunnerServiceBase
    {
        app.Logger.LogInformation("Starting Application");

        app.MapGrpcService<TService>();
    }
}
