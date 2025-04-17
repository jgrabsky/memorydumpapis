// ServerApi/Program.cs
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace ServerApi // Make sure this matches your project's namespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    var server = services.GetRequiredService<IServer>();
                    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses.ToList();
                    if (addresses != null && addresses.Any())
                    {
                        Console.WriteLine($"Server starting and listening on: {string.Join(", ", addresses)}");
                    }
                    else
                    {
                        Console.WriteLine("Server starting, listening address could not be determined.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while trying to determine server address: {ex.Message}");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}