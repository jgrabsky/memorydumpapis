// ServerApi/Program.cs
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration; // Add this namespace
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net;

namespace ServerApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run(); // Run the host first

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var server = services.GetRequiredService<IServer>();
                    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses.ToList();
                    if (addresses != null && addresses.Any(a => a.StartsWith("https", StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine($"Server is now listening on HTTPS: {string.Join(", ", addresses.Where(a => a.StartsWith("https", StringComparison.OrdinalIgnoreCase)))}");
                    }
                    else
                    {
                        Console.WriteLine("Server is running, but no HTTPS address could be determined.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred after running while trying to determine server address: {ex.Message}");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        var configuration = context.Configuration;
                        string certificatePath = configuration["Kestrel:Certificates:Default:Path"];
                        string certificatePassword = configuration["Kestrel:Certificates:Default:Password"];

                        if (!string.IsNullOrEmpty(certificatePath) && !string.IsNullOrEmpty(certificatePassword))
                        {
                            options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                            {
                                listenOptions.UseHttps(certificatePath, certificatePassword);
                            });
                        }
                        else
                        {
                            Console.WriteLine("Warning: HTTPS certificate path or password not configured via environment variables or configuration.");
                            // You might want to throw an exception or log more severely in production
                        }

                        // Optionally, remove the HTTP endpoint if you want HTTPS only
                        // options.Listen(IPAddress.Loopback, 5000);
                    });
                });
    }
}