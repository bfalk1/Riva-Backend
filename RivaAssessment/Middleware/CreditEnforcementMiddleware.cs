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
        // TODO: Implement the middleware
        // - Extract user ID from X-User-Id header
        // - Call creditService.TryDeductCreditAsync() to deduct credit
        // - Return 402 if no credits available
        // - Allow request to proceed if credit deducted
        
        await _next(context);
    }
}
