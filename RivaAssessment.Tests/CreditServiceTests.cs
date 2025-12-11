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

    // TODO: Implement tests for CreditService
}
