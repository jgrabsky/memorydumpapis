// ServerApi/Controllers/DocumentController.cs
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(ILogger<DocumentController> logger)
    {
        _logger = logger;
    }

    [HttpPost("upload")]
    public IActionResult UploadDocument([FromBody] DocumentPayload document)
    {
        _logger.LogInformation($"Received document: {document.FileName}");

        // Simulate processing the document
        _logger.LogInformation($"Document Content (Server-Side): {document.Content}");

        // Trigger Memory Dump (FOR DEVELOPMENT/DEBUGGING ONLY!)
        TriggerMemoryDumpWithPayload(document);

        return Ok("Document received and processed (memory dump triggered).");
    }

    private void TriggerMemoryDumpWithPayload(DocumentPayload payload)
    {
        try
        {
            string dumpFileName = $"memory_dump_{DateTime.Now:yyyyMMdd_HHmmss}.dmp";
            string dumpPath = Path.Combine(Directory.GetCurrentDirectory(), dumpFileName);

            // Include the payload information in the dump's description (this might not be directly visible in all dump analysis tools)
            string description = $"Document Payload: {JsonSerializer.Serialize(payload)}";

            using (Process currentProcess = Process.GetCurrentProcess())
            {
                // This method of triggering a full user-mode dump is platform-specific and might require elevated privileges.
                // Consider using tools like ProcDump (from Sysinternals) for more reliable and configurable dumps.

                // WARNING: This is a simplified approach and might not capture the exact memory state you expect.
                // For more control, consider using libraries like MiniDumpWriteDump (requires interop).

                // This example attempts to signal the process to create a dump.
                // The actual dump creation behavior might depend on OS settings and debugging tools.
                // It's more common to use external tools for reliable memory dumps.

                // For demonstration purposes, we'll just log the payload.
                _logger.LogWarning($"Attempting to trigger memory dump with payload: {description}");

                // In a real debugging scenario, you'd likely use an external tool
                // attached to the process or configure the OS to create dumps on certain events.
            }

            _logger.LogWarning($"Memory dump attempted: {dumpPath}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error triggering memory dump: {ex.Message}");
        }
    }
}