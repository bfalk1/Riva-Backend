namespace RivaAssessment.Repositories;

/// <summary>
/// Interface for the legacy billing repository.
/// This repository simulates a slow database with ~100ms latency per operation.
/// </summary>
public interface ILegacyBillingRepository
{
    /// <summary>
    /// Gets the current credit balance for a user.
    /// Simulated latency: ~100ms
    /// </summary>
    Task<int> GetCreditsAsync(string userId);

    /// <summary>
    /// Deducts credits from a user's account.
    /// Simulated latency: ~100ms
    /// </summary>
    /// <returns>True if deduction was successful, false if insufficient credits</returns>
    Task<bool> DeductCreditsAsync(string userId, int amount);
}


