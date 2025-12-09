namespace RivaAssessment.Services;

/// <summary>
/// Service for managing user credits with caching and concurrency control.
/// This service must:
/// - Use caching to avoid calling LegacyBillingRepository on every request (20ms latency requirement)
/// - Handle race conditions (atomic operations)
/// - Ensure only one request succeeds when user has 1 credit and 10 concurrent requests arrive
/// </summary>
public interface ICreditService
{
    /// <summary>
    /// Attempts to deduct 1 credit from the user's account.
    /// This operation must be atomic and handle concurrency.
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <returns>True if credit was successfully deducted, false if insufficient credits</returns>
    Task<bool> TryDeductCreditAsync(string userId);
}


