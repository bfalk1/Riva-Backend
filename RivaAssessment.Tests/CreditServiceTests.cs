using Microsoft.Extensions.Logging;
using Moq;
using RivaAssessment.Repositories;
using RivaAssessment.Services;
using Xunit;

namespace RivaAssessment.Tests;

/// <summary>
/// Tests for the Credit Service
/// </summary>
public class CreditServiceTests
{
    private readonly Mock<ILegacyBillingRepository> _repositoryMock;
    private readonly Mock<ILogger<CreditService>> _loggerMock;

    public CreditServiceTests()
    {
        _repositoryMock = new Mock<ILegacyBillingRepository>();
        _loggerMock = new Mock<ILogger<CreditService>>();
    }

    [Fact]
    public async Task TryDeductCredit_ReturnsFalse_WhenUserHasNoCredits()
    {
        // Arrange
        // Mock the repository to return 0 credits for user3
        // Note: Depending on your caching implementation, you may need to:
        // - Setup GetCreditsAsync to return 0
        // - Setup DeductCreditsAsync to return false (if your service calls it)
        _repositoryMock.Setup(x => x.GetCreditsAsync("user3"))
            .ReturnsAsync(0);

        var service = new CreditService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        var result = await service.TryDeductCreditAsync("user3");

        // Assert
        // Verify that the service returns false when user has no credits
        Assert.False(result);
        
        // Optional: Verify repository was called (adjust based on your implementation)
        // _repositoryMock.Verify(x => x.GetCreditsAsync("user3"), Times.Once);
    }

    [Fact]
    public async Task ConcurrentDeductions_WithOneCredit_OnlyOneSucceeds()
    {
        // This is the critical concurrency test!
        // It verifies that when a user has 1 credit and 10 threads
        // try to deduct simultaneously, only 1 succeeds (prevents double-spending).
        
        // Arrange
        var userId = "user1";
        
        // Mock the repository to return 1 credit initially
        // Note: Your caching implementation should handle this correctly
        // If your service calls GetCreditsAsync multiple times, you may need:
        // - SetupSequence for different return values
        // - Or setup to return 1, then 0 after first deduction
        _repositoryMock.Setup(x => x.GetCreditsAsync(userId))
            .ReturnsAsync(1);

        var service = new CreditService(_repositoryMock.Object, _loggerMock.Object);

        // Act
        // Create 10 concurrent tasks that all try to deduct 1 credit simultaneously
        // This simulates 10 parallel HTTP requests hitting the API at the same time
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => service.TryDeductCreditAsync(userId))
            .ToArray();
        
        // Wait for all tasks to complete
        var results = await Task.WhenAll(tasks);

        // Assert
        // CRITICAL: Exactly 1 request should succeed (return true)
        // The other 9 should fail (return false)
        // This proves your concurrency control is working correctly
        var successCount = results.Count(r => r);
        var failureCount = results.Count(r => !r);
        
        Assert.Equal(1, successCount);
        Assert.Equal(9, failureCount);
        
        // Optional: Verify the repository was called appropriately
        // The exact number of calls depends on your caching strategy
        // _repositoryMock.Verify(x => x.GetCreditsAsync(userId), Times.AtLeastOnce);
    }
}

