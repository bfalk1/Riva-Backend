# Technical Assessment: Senior Backend Developer (C#/.NET)
### Project: The Credit Enforcement Engine

**Role:** Senior Backend Developer  
**Time Limit:** 60 - 90 Minutes  
**Tech Stack:** C# (.NET 9), ASP.NET Core, xUnit  
**Focus:** Concurrency, Performance Optimization, Middleware Architecture  

---

## 1. Scenario & Context
You are a Senior Backend Engineer at Riva. We are pivoting to a "Pre-Paid Credit" model for our API to better monetize our services.

**The Problem:** We currently rely on a legacy database to track customer credits. It is reliable but slow (~100ms latency per read/write). We need a **high-performance Middleware layer** that sits in front of our API to enforce billing limits in real-time.

**The "Ask" from Leadership:** > "Build a robust enforcement engine that deducts 1 credit per request. If a user hits 0 credits, block them immediately with a `402 Payment Required`. This system must handle thousands of concurrent requests without race conditions (double-spending) and must add less than **20ms** of latency to the pipeline."

---

## 2. Technical Requirements

### A. The Middleware (The Gatekeeper)
* Create an ASP.NET Core Middleware component (`CreditEnforcementMiddleware`).
* It must intercept every HTTP request.
* If the user has credits: Deduct 1 and allow the request to proceed.
* If the user has **0** credits: Short-circuit the pipeline and return `402 Payment Required`.

### B. The Concurrency Challenge (Race Conditions)
* **Scenario:** A user has **1 Credit** remaining. They fire **10 parallel requests** simultaneously.
* **Requirement:** Only **ONE** request must succeed. The other 9 must fail.
* **Constraint:** You cannot rely solely on the database for this check due to the latency requirement. You must implement a strategy to handle this "Double-Spend" problem efficiently (e.g., locking, atomic operations, or optimistic concurrency).

### C. Performance (The 20ms Limit)
* The provided `LegacyBillingRepository` has a simulated latency of **100ms**.
* You **cannot** call this repository directly on every request. Implement a caching strategy (e.g., "Token Bucket" or "Cache-Aside") to satisfy the speed requirement while ensuring eventual consistency.

### D. Testing (Critical)
* You must implement and verify your logic with **Unit Tests**.
* **Test Case 1:** Implement `TryDeductCredit_ReturnsFalse_WhenUserHasNoCredits` - Verify that the service returns `false` when a user has 0 credits.
* **Test Case 2 (The Stress Test):** Implement `ConcurrentDeductions_WithOneCredit_OnlyOneSucceeds` - Verify that 10 concurrent requests for a user with 1 credit results in exactly **1 Success** and **9 Failures**.
* The test structure is provided in `RivaAssessment.Tests/CreditServiceTests.cs`, but you must complete the implementations and ensure all tests pass.

---

## 3. Starter Code & Setup Instructions

### Project Structure
The solution is already set up with:
- **RivaAssessment**: Main API project (.NET 9)
- **RivaAssessment.Tests**: Test project with xUnit (.NET 9)
- **LegacyBillingRepository**: Provided repository that simulates slow database (100ms latency)
- **CreditService**: Service class that needs implementation
- **CreditEnforcementMiddleware**: Middleware class that needs completion
- **Test files**: Test structure provided, needs completion

### Initial Setup
See [SETUP.md](SETUP.md) for detailed setup instructions. Quick start:

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests (some will fail initially - this is expected)
dotnet test
```

## 4. What You Need to Implement

### CreditService (`RivaAssessment/Services/CreditService.cs`)
- **Status:** Currently throws `NotImplementedException`
- **Task:** Implement `TryDeductCreditAsync(string userId)` method
- **Requirements:**
  - Implement caching to avoid 100ms database calls
  - Add concurrency control (locking/atomic operations)
  - Ensure < 20ms latency
  - Return `true` if credit deducted, `false` if insufficient credits

### CreditEnforcementMiddleware (`RivaAssessment/Middleware/CreditEnforcementMiddleware.cs`)
- **Status:** Partially implemented
- **Task:** Complete the middleware implementation
- **Requirements:**
  - Call `CreditService.TryDeductCreditAsync()`
  - Return `402 Payment Required` if no credits
  - Allow request to proceed if credit deducted
  - Register in `Program.cs`

### Tests (`RivaAssessment.Tests/CreditServiceTests.cs`)
- **Status:** Test structure provided, needs completion
- **Task:** Complete test implementations
- **Requirements:**
  - Complete `TryDeductCredit_ReturnsFalse_WhenUserHasNoCredits` test
  - Complete `ConcurrentDeductions_WithOneCredit_OnlyOneSucceeds` test
  - Ensure all tests pass

For detailed implementation guidance, see [ASSIGNMENT.md](ASSIGNMENT.md).

## 5. Success Criteria

Your implementation is complete when:
1. All tests pass (`dotnet test` shows 8/8 passing)
2. `CreditService.TryDeductCreditAsync()` is fully implemented
3. Middleware is registered and working
4. API blocks users with 0 credits (returns 402)
5. Concurrency test passes: 10 requests with 1 credit = 1 success, 9 failures
6. Response time is < 20ms (caching prevents 100ms database calls)

## 6. Testing

### Run Tests
```bash
dotnet test
```

### Test the API
1. Start the API: `cd RivaAssessment && dotnet run`
2. Open Swagger: http://localhost:5000/swagger
3. Test with `X-User-Id` header:
   - `user1` (has credits) - should succeed
   - `user3` (no credits) - should return 402

### Test Users
- `user1`: 10 credits
- `user2`: 5 credits
- `user3`: 0 credits (should always return 402)
- `user4`: 100 credits