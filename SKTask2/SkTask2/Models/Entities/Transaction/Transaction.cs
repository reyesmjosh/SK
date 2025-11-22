public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string CustomerEmail { get; set; }
    public string Status { get; set; } = "pending"; // pending, succeeded, failed
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
}