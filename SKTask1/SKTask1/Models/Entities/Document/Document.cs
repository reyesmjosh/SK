public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public int Version { get; set; } = 1;
    public long Size { get; set; }
    public string ContentType { get; set; }
    public DateTime UploadTimestamp { get; set; } = DateTime.UtcNow;
    public string StoragePath { get; set; }
    public string DownloadLink { get; set; }
}