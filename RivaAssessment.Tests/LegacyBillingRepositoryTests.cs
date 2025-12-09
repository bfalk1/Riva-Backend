using RivaAssessment.Repositories;
using Xunit;

namespace RivaAssessment.Tests;

/// <summary>
/// Tests for the Legacy Billing Repository
/// These tests verify the repository behavior (simulated latency, etc.)
/// </summary>
public class LegacyBillingRepositoryTests
{
    [Fact]
    public async Task GetCreditsAsync_ReturnsCorrectCredits()
    {
        // Arrange
        var repository = new LegacyBillingRepository();
        var userId = "user1";

        // Act
        var credits = await repository.GetCreditsAsync(userId);

        // Assert
        Assert.Equal(10, credits);
    }

    [Fact]
    public async Task DeductCreditsAsync_ReturnsFalse_WhenInsufficientCredits()
    {
        // Arrange
        var repository = new LegacyBillingRepository();
        var userId = "user3"; // This user has 0 credits

        // Act
        var result = await repository.DeductCreditsAsync(userId, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeductCreditsAsync_ReturnsTrue_WhenSufficientCredits()
    {
        // Arrange
        var repository = new LegacyBillingRepository();
        var userId = "user1"; // This user has 10 credits

        // Act
        var result = await repository.DeductCreditsAsync(userId, 1);

        // Assert
        Assert.True(result);
        
        // Verify credits were actually deducted
        var remainingCredits = await repository.GetCreditsAsync(userId);
        Assert.Equal(9, remainingCredits);
    }
}

