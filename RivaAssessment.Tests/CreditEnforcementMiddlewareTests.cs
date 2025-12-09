using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using RivaAssessment.Middleware;
using RivaAssessment.Services;
using System.Net;
using Xunit;

namespace RivaAssessment.Tests;

/// <summary>
/// Tests for the Credit Enforcement Middleware
/// </summary>
public class CreditEnforcementMiddlewareTests
{
    private readonly Mock<ICreditService> _creditServiceMock;
    private readonly Mock<ILogger<CreditEnforcementMiddleware>> _loggerMock;
    private readonly RequestDelegate _next;

    public CreditEnforcementMiddlewareTests()
    {
        _creditServiceMock = new Mock<ICreditService>();
        _loggerMock = new Mock<ILogger<CreditEnforcementMiddleware>>();
        _next = (HttpContext context) => Task.CompletedTask;
    }

    [Fact]
    public async Task Middleware_BlocksUser_WhenNoCreditsAvailable()
    {
        // Arrange
        var userId = "user3"; // This user has 0 credits in the repository
        _creditServiceMock.Setup(x => x.TryDeductCreditAsync(userId))
            .ReturnsAsync(false);

        var middleware = new CreditEnforcementMiddleware(_next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Headers["X-User-Id"] = userId;

        // Act
        await middleware.InvokeAsync(context, _creditServiceMock.Object);

        // Assert
        Assert.Equal(402, context.Response.StatusCode);
    }

    [Fact]
    public async Task Middleware_AllowsRequest_WhenCreditsAvailable()
    {
        // Arrange
        var userId = "user1"; // This user has credits
        _creditServiceMock.Setup(x => x.TryDeductCreditAsync(userId))
            .ReturnsAsync(true);

        var context = new DefaultHttpContext();
        context.Request.Headers["X-User-Id"] = userId;
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) => { nextCalled = true; return Task.CompletedTask; };
        var middlewareWithNext = new CreditEnforcementMiddleware(next, _loggerMock.Object);

        // Act
        await middlewareWithNext.InvokeAsync(context, _creditServiceMock.Object);

        // Assert
        Assert.NotEqual(402, context.Response.StatusCode);
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task ConcurrentRequests_WithOneCredit_OnlyOneSucceeds()
    {
        // TODO: Implement this test
        // This is the critical stress test:
        // - User has 1 credit
        // - Fire 10 concurrent requests
        // - Only 1 should succeed, 9 should fail with 402
        
        // Arrange
        // var userId = "user1";
        // var creditService = new Mock<ICreditService>();
        
        // TODO: Set up the credit service to handle concurrency properly
        // This test will verify that your implementation prevents race conditions
        
        // Act
        // TODO: Fire 10 concurrent requests
        
        // Assert
        // TODO: Verify exactly 1 success and 9 failures
        
        // Placeholder to make test compile
        await Task.CompletedTask;
    }
}

