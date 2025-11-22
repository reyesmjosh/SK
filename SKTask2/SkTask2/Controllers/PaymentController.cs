using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentController(AppDbContext db) => _db = db;

    [HttpPost("create-session")]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest req)
    {
        var transaction = new Transaction
        {
            Amount = req.Amount,
            CustomerEmail = req.CustomerEmail ?? "test@example.com",
            Currency = req.Currency ?? "USD"
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var request = HttpContext.Request;
        var paymentUrl = $"{request.Scheme}://{request.Host}/api/payment/checkout/{transaction.SessionId}";

        return Ok(new
        {
            SessionId = transaction.SessionId,
            PaymentUrl = paymentUrl,
            Status = "pending"
        });
    }

    [HttpGet("checkout/{sessionId}")]
    public IActionResult Checkout(string sessionId)
    {
        var transaction = _db.Transactions.FirstOrDefault(t => t.SessionId == sessionId);
        if (transaction == null) return NotFound("Session not found");

        var html = $@"
        <!DOCTYPE html>
        <html><body style='font-family:Arial; text-align:center; margin-top:100px;'>
            <h2>Mock Payment - {transaction.Amount:0.00} {transaction.Currency}</h2>
            <p>Email: {transaction.CustomerEmail}</p>
            <form method='post' action='/api/payment/process/{sessionId}'>
                <button type='submit' name='result' value='success' style='background:green; color:white; padding:15px; margin:10px; font-size:18px;'>
                    Pay Successfully
                </button>
                <button type='submit' name='result' value='failed' style='background:red; color:white; padding:15px; margin:10px; font-size:18px;'>
                    Simulate Failure
                </button>
            </form>
        </body></html>";

        return Content(html, "text/html");
    }

    [HttpPost("process/{sessionId}")]
    public async Task<IActionResult> Process(string sessionId, [FromForm] string result)
    {
        var transaction = await _db.Transactions.FirstOrDefaultAsync(t => t.SessionId == sessionId);
        if (transaction == null) return NotFound();

        if (result == "success")
        {
            transaction.Status = "succeeded";
            transaction.ProcessedAt = DateTime.UtcNow;
        }
        else
        {
            transaction.Status = "failed";
            transaction.FailureReason = "Card declined (mock)";
            transaction.ProcessedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        _ = Task.Run(() => TriggerWebhook(transaction));

        return Redirect($"/api/payment/result/{sessionId}");
    }

    [HttpGet("result/{sessionId}")]
    public IActionResult Result(string sessionId)
    {
        var t = _db.Transactions.FirstOrDefault(x => x.SessionId == sessionId);
        if (t == null) return NotFound();

        var color = t.Status == "succeeded" ? "green" : "red";
        return Content($@"
            <h1 style='color:{color}; text-align:center; margin-top:100px;'>
                Payment {t.Status.ToUpper()}!
            </h1>
            <p style='text-align:center;'>Session: {sessionId}</p>
            <script>setTimeout(() => window.close(), 5000);</script>", "text/html");
    }

    [HttpPost("webhook")]
    public IActionResult Webhook([FromBody] object payload)
    {
        Console.WriteLine("WEBHOOK RECEIVED:");
        Console.WriteLine(payload?.ToString());
        return Ok(new { status = "received" });
    }

    private async Task TriggerWebhook(Transaction t)
    {
        var webhookUrl = "http://localhost:5000/api/payment/webhook"; // Dev. note: Change to app URL
        var payload = new
        {
            event_type = "payment." + t.Status,
            session_id = t.SessionId,
            amount = t.Amount,
            currency = t.Currency,
            customer_email = t.CustomerEmail,
            status = t.Status,
            timestamp = DateTime.UtcNow
        };

        using var client = new HttpClient();
        try
        {
            await client.PostAsJsonAsync(webhookUrl, payload);
        }
        catch { /*Ignore*/ }
    }

    public class CreateSessionRequest
    {
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? CustomerEmail { get; set; }
    }
}