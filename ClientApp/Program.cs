// ClientApp/Program.cs
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class DocumentPayload
{
    public string? FileName { get; set; }
    public string? Content { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        string serverUrl = "https://localhost:5001/document/upload"; // Changed to HTTPS and the configured port

        var document = new DocumentPayload
        {
            FileName = "my_secure_document.txt",
            Content = "This is the content sent over HTTPS."
        };

        using (var httpClient = new HttpClient())
        {
            try
            {
                // For self-signed certificates in development, you might need to bypass certificate validation
                // WARNING: Do not do this in production!
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    // Bypass certificate validation (only for development with self-signed certs)
                    return true;
                };
                using var insecureClient = new HttpClient(handler);

                var response = await insecureClient.PostAsJsonAsync(serverUrl, document);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Document uploaded successfully over HTTPS. Check server logs for memory dump attempt.");
                }
                else
                {
                    Console.WriteLine($"Error uploading document over HTTPS: {response.StatusCode}");
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}