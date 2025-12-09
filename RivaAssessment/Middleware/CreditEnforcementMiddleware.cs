using RivaAssessment.Services;

namespace RivaAssessment.Middleware;

/// <summary>
/// Middleware that enforces credit limits on every request.
/// 
/// Requirements:
/// - Intercept every HTTP request
/// - If user has credits: Deduct 1 and allow request to proceed
/// - If user has 0 credits: Return 402 Payment Required
/// - Must handle concurrency (race conditions)
/// - Must add less than 20ms latency
/// </summary>
public class CreditEnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CreditEnforcementMiddleware> _logger;

    public CreditEnforcementMiddleware(
        RequestDelegate next,
        ILogger<CreditEnforcementMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICreditService creditService)
    {
        // TODO: Extract user ID from request (e.g., from headers, claims, etc.)
        // For now, using a simple header approach
        var userId = context.Request.Headers["X-User-Id"].FirstOrDefault() ?? "user1";

        // TODO: Implement credit check and deduction
        // This should:
        // 1. Check if user has credits (using caching to avoid 100ms latency)
        // 2. Atomically deduct 1 credit
        // 3. Handle race conditions (only 1 of 10 concurrent requests should succeed if user has 1 credit)
        // 4. Return 402 if no credits available

        // Placeholder - replace with actual implementation
        var hasCredits = await creditService.TryDeductCreditAsync(userId);
        
        if (!hasCredits)
        {
            context.Response.StatusCode = 402; // Payment Required
            await context.Response.WriteAsync("Payment Required - Insufficient credits");
            return;
        }

        // User has credits, proceed with the request
        await _next(context);
    }
}


