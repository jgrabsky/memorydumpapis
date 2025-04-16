// ClientApp/Program.cs
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class DocumentPayload
{
    public string FileName { get; set; }
    public string Content { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        string serverUrl = "http://localhost:5000/document/upload"; // Adjust if your server runs on a different port

        var document = new DocumentPayload
        {
            FileName = "my_document.txt",
            Content = "This is the content of my test document. Some sensitive data might be here."
        };

        using (var httpClient = new HttpClient())
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync(serverUrl, document);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Document uploaded successfully. Check server logs for memory dump attempt.");
                }
                else
                {
                    Console.WriteLine($"Error uploading document: {response.StatusCode}");
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