using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DocumentsController(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        // Check for existing file with same name to handle versioning
        var existingDocs = await _context.Documents
            .Where(d => d.FileName == file.FileName)
            .OrderByDescending(d => d.Version)
            .ToListAsync();

        int newVersion = 1;
        if (existingDocs.Any())
        {
            newVersion = existingDocs.First().Version + 1;
        }

        // Generate unique storage path (e.g., Storage/filename_vX.ext)
        var extension = Path.GetExtension(file.FileName);
        var storageFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_v{newVersion}{extension}";
        var storagePath = Path.Combine(_environment.ContentRootPath, "Storage", storageFileName);

        // Save file to disk
        using (var stream = new FileStream(storagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Generate download link (assuming the app runs on localhost:5001 for dev; adjust for production)
        var downloadLink = $"{Request.Scheme}://{Request.Host}/api/documents/download/{storageFileName}";

        // Store metadata
        var document = new Document
        {
            FileName = file.FileName,
            Version = newVersion,
            Size = file.Length,
            ContentType = file.ContentType,
            StoragePath = storagePath,
            DownloadLink = downloadLink
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "File uploaded successfully.",
            DownloadLink = downloadLink,
            Version = newVersion,
            Metadata = new { document.Size, document.ContentType, document.UploadTimestamp }
        });
    }

    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> Download(string fileName)
    {
        var filePath = Path.Combine(_environment.ContentRootPath, "Storage", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound("File not found.");

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        var contentType = "application/octet-stream"; // Default; can query DB for exact type if needed

        return File(fileBytes, contentType, fileName);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var documents = await _context.Documents.ToListAsync();
        return Ok(documents.Select(d => new
        {
            d.FileName,
            d.Version,
            d.Size,
            d.ContentType,
            d.UploadTimestamp,
            d.DownloadLink
        }));
    }

    [HttpGet("{fileName}/versions")]
    public async Task<IActionResult> GetVersions(string fileName)
    {
        var versions = await _context.Documents
            .Where(d => d.FileName == fileName)
            .OrderBy(d => d.Version)
            .ToListAsync();

        if (!versions.Any())
            return NotFound("No versions found.");

        return Ok(versions.Select(d => new
        {
            d.Version,
            d.UploadTimestamp,
            d.DownloadLink
        }));
    }
}